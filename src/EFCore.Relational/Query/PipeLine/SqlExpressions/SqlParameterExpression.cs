﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Relational.Query.PipeLine.SqlExpressions
{
    public class SqlParameterExpression : SqlExpression
    {
        private readonly ParameterExpression _parameterExpression;

        public SqlParameterExpression(ParameterExpression parameterExpression, RelationalTypeMapping typeMapping)
            : base(parameterExpression.Type, typeMapping, false, true)
        {
            _parameterExpression = parameterExpression;
        }

        private SqlParameterExpression(
            ParameterExpression parameterExpression, RelationalTypeMapping typeMapping, bool treatAsValue)
            : base(parameterExpression.Type, typeMapping, false, treatAsValue)
        {
            _parameterExpression = parameterExpression;
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return this;
        }

        public override SqlExpression ConvertToValue(bool treatAsValue)
        {
            return new SqlParameterExpression(_parameterExpression, TypeMapping, treatAsValue);
        }

        public string Name => _parameterExpression.Name;

        public override bool Equals(object obj)
            => obj != null
            && (ReferenceEquals(this, obj)
                || obj is SqlParameterExpression sqlParameterExpression
                    && Equals(sqlParameterExpression));

        private bool Equals(SqlParameterExpression sqlParameterExpression)
            => base.Equals(sqlParameterExpression)
            && string.Equals(Name, sqlParameterExpression.Name);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ Name.GetHashCode();

                return hashCode;
            }
        }
    }
}