﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Relational.Query.PipeLine.SqlExpressions
{
    public class SqlNullExpression : SqlExpression
    {
        public SqlNullExpression(
            SqlExpression operand,
            bool negated,
            Type type,
            RelationalTypeMapping typeMapping,
            bool condition)
            : base(type, typeMapping, condition)
        {
            Check.NotNull(operand, nameof(operand));

            Operand = operand;
            Negated = negated;
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var operand = (SqlExpression)visitor.Visit(Operand);

            return operand != Operand
                ? new SqlNullExpression(operand, Negated, Type, TypeMapping, IsCondition)
                : this;
        }

        public SqlExpression Operand { get; }
        public bool Negated { get; }
    }
}
