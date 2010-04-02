// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyExpressionEvaluator.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation.
// </copyright>
// <summary>
//   Code to evaluate a predicate Expression and determine
//   a key range which contains all items matched by the predicate.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Isam.Esent.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Contains methods to evaluate a predicate Expression and determine
    /// a key range which contains all items matched by the predicate.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    internal static class KeyExpressionEvaluator<TKey> where TKey : IComparable<TKey>
    {
        /// <summary>
        /// Evaluate a predicate Expression and determine a key range which
        /// contains all items matched by the predicate.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="keyMemberName">The name of the parameter member that is the key.</param>
        /// <returns>
        /// A KeyRange that contains all items matched by the predicate. If no
        /// range can be determined the range will include all items.
        /// </returns>
        public static KeyRange<TKey> GetKeyRange(Expression expression, string keyMemberName)
        {
            if (null == expression)
            {
                throw new ArgumentNullException("expression");    
            }

            if (null == keyMemberName)
            {
                throw new ArgumentNullException("keyMemberName");
            }

            return UnionRanges(GetKeyRangesOfSubtree(expression, keyMemberName));
        }

        /// <summary>
        /// Evaluate a predicate Expression and determine key ranges which
        /// contain all items matched by the predicate.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="keyMemberName">The name of the parameter member that is the key.</param>
        /// <returns>
        /// A list of KeyRanges containing all items matched by the predicate. If no
        /// range can be determined the ranges will include all items.
        /// </returns>
        private static IEnumerable<KeyRange<TKey>> GetKeyRangesOfSubtree(Expression expression, string keyMemberName)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.AndAlso:
                {
                    // Intersect the union of the left and right parts
                    var binaryExpression = (BinaryExpression) expression;
                    KeyRange<TKey> left = UnionRanges(GetKeyRangesOfSubtree(binaryExpression.Left, keyMemberName));
                    KeyRange<TKey> right = UnionRanges(GetKeyRangesOfSubtree(binaryExpression.Right, keyMemberName));
                    return new KeyRange<TKey>[] { left & right };
                }

                case ExpressionType.OrElse:
                {
                    // Return a list of the left and right parts
                    var binaryExpression = (BinaryExpression) expression;
                    return
                        GetKeyRangesOfSubtree(binaryExpression.Left, keyMemberName).Concat(
                            GetKeyRangesOfSubtree(binaryExpression.Right, keyMemberName));
                }

                case ExpressionType.Not:
                {
                    // Here we apply DeMorgan's Law: a NOT of a set of ranges becomes
                    // the intersection of their negations.
                    var unaryExpression = (UnaryExpression) expression;
                    IEnumerable<KeyRange<TKey>> invertedRanges =
                        from r in GetKeyRangesOfSubtree(unaryExpression.Operand, keyMemberName)
                        select r.Invert();
                    return new KeyRange<TKey>[] { IntersectRanges(invertedRanges) };
                }

                case ExpressionType.Equal:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                {
                    // Return a range
                    var binaryExpression = (BinaryExpression) expression;
                    TKey value;
                    ExpressionType expressionType;
                    if (IsConstantComparison(binaryExpression, keyMemberName, out value, out expressionType))
                    {
                        switch (expressionType)
                        {
                            case ExpressionType.Equal:
                                var key = new Key<TKey>(value, true);
                                return new KeyRange<TKey>[] { new KeyRange<TKey>(key, key) };
                            case ExpressionType.LessThan:
                                return new KeyRange<TKey>[] { new KeyRange<TKey>(null, new Key<TKey>(value, false)) };
                            case ExpressionType.LessThanOrEqual:
                                return new KeyRange<TKey>[] { new KeyRange<TKey>(null, new Key<TKey>(value, true)) };
                            case ExpressionType.GreaterThan:
                                return new KeyRange<TKey>[] { new KeyRange<TKey>(new Key<TKey>(value, false), null) };
                            case ExpressionType.GreaterThanOrEqual:
                                return new KeyRange<TKey>[] { new KeyRange<TKey>(new Key<TKey>(value, true), null) };
                            default:
                                throw new InvalidOperationException(expressionType.ToString());
                        }
                    }

                    break;
                }

                default:
                    break;
            }

            return new List<KeyRange<TKey>> { KeyRange<TKey>.OpenRange };
        }

        /// <summary>
        /// Calculate the union of a set of key ranges.
        /// </summary>
        /// <param name="ranges">The key ranges to union.</param>
        /// <returns>A union of all the ranges.</returns>
        private static KeyRange<TKey> UnionRanges(IEnumerable<KeyRange<TKey>> ranges)
        {
            return ranges.Aggregate(ranges.First(), (a, b) => a | b);
        }

        /// <summary>
        /// Calculate the intersection of a set of key ranges.
        /// </summary>
        /// <param name="ranges">The key ranges to union.</param>
        /// <returns>A union of all the ranges.</returns>
        private static KeyRange<TKey> IntersectRanges(IEnumerable<KeyRange<TKey>> ranges)
        {
            return ranges.Aggregate(ranges.First(), (a, b) => a & b);
        }

        /// <summary>
        /// Determine if the current binary expression involves the Key of the parameter
        /// and a constant value.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="keyMemberName">The name of the parameter member that is the key.</param>
        /// <param name="value">Returns the value being compared to the key.</param>
        /// <param name="expressionType">Returns the type of the expression.</param>
        /// <returns>
        /// True if the expression involves the key of the parameter and a constant value.
        /// </returns>
        private static bool IsConstantComparison(BinaryExpression expression, string keyMemberName, out TKey value, out ExpressionType expressionType)
        {
            if (IsKeyAccess(expression.Left, keyMemberName)
                && ConstantExpressionEvaluator<TKey>.TryGetConstantExpression(expression.Right, out value))
            {
                expressionType = expression.NodeType;
                return true;
            }

            if (IsKeyAccess(expression.Right, keyMemberName)
                && ConstantExpressionEvaluator<TKey>.TryGetConstantExpression(expression.Left, out value))
            {
                // The access is on the right so we have to switch the comparison type
                expressionType = GetReverseExpressionType(expression.NodeType);
                return true;
            }

            value = default(TKey);
            expressionType = default(ExpressionType);
            return false;
        }

        /// <summary>
        /// Reverse the type of the comparison. This is used when the key is on the right hand side
        /// of the comparison, so that 3 LT Key becomes Key GT 3.
        /// </summary>
        /// <param name="originalExpressionType">The original expression type.</param>
        /// <returns>The reverse of a comparison expression type or the original expression type.</returns>
        private static ExpressionType GetReverseExpressionType(ExpressionType originalExpressionType)
        {
            ExpressionType expressionType;
            switch (originalExpressionType)
            {
                case ExpressionType.LessThan:
                    expressionType = ExpressionType.GreaterThan;
                    break;
                case ExpressionType.LessThanOrEqual:
                    expressionType = ExpressionType.GreaterThanOrEqual;
                    break;
                case ExpressionType.GreaterThan:
                    expressionType = ExpressionType.LessThan;
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    expressionType = ExpressionType.LessThanOrEqual;
                    break;
                default:
                    expressionType = originalExpressionType;
                    break;
            }

            return expressionType;
        }

        /// <summary>
        /// Determine if the expression is accessing the key of the paramter of the expression.
        /// parameter.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <param name="keyMemberName">The name of the parameter member that is the key.</param>
        /// <returns>True if the expression is accessing the key of the parameter.</returns>
        private static bool IsKeyAccess(Expression expression, string keyMemberName)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                var member = (MemberExpression) expression;
                if (
                    null != member.Expression
                    && member.Expression.NodeType == ExpressionType.Parameter
                    && member.Member.Name == keyMemberName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}