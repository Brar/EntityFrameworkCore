﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Relational.Query.PipeLine.SqlExpressions;

namespace Microsoft.EntityFrameworkCore.Relational.Query.PipeLine
{
    public class EqualsTranslator : IMethodCallTranslator
    {
        public EqualsTranslator()
        {
        }


        public SqlExpression Translate(SqlExpression instance, MethodInfo method, IList<SqlExpression> arguments)
        {
            SqlExpression left = null;
            SqlExpression right = null;

            if (method.Name == nameof(object.Equals)
                && instance != null
                && arguments.Count == 1)
            {
                left = instance;
                right = RemoveObjectConvert(arguments[0]);
            }
            else if (method.Name == nameof(object.Equals)
                && arguments.Count == 2
                && arguments[0].Type == arguments[1].Type)
            {
                left = RemoveObjectConvert(arguments[0]);
                right = RemoveObjectConvert(arguments[1]);
            }

            if (left != null && right != null)
            {
                if (left.Type.UnwrapNullableType() == right.Type.UnwrapNullableType())
                {
                    return new SqlBinaryExpression(
                        ExpressionType.Equal,
                        left,
                        right,
                        typeof(bool),
                        null,
                        true);
                }
                else
                {
                    return new SqlConstantExpression(Expression.Constant(false), null, true);
                }
            }

            return null;
        }

        private SqlExpression RemoveObjectConvert(SqlExpression expression)
        {
            if (expression is SqlCastExpression sqlCast
                && sqlCast.Type == typeof(object))
            {
                return sqlCast.Operand;
            }

            return expression;
        }
    }
}
