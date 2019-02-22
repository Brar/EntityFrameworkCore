﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Relational.Query.PipeLine.SqlExpressions
{
    public class ColumnExpression : SqlExpression
    {
        private readonly IProperty _property;

        public ColumnExpression(IProperty property, TableExpressionBase table)
            : base(property.ClrType, property.FindRelationalMapping(), false, true)
        {
            _property = property;
            Table = table;
        }

        private ColumnExpression(IProperty property, TableExpressionBase table, bool treatAsValue)
            : base(property.ClrType, property.FindRelationalMapping(), false, treatAsValue)
        {
            _property = property;
            Table = table;
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var newTable = (TableExpressionBase)visitor.Visit(Table);

            return newTable != Table
                ? new ColumnExpression(_property, newTable)
                : this;
        }

        public override SqlExpression ConvertToValue(bool treatAsValue)
        {
            return new ColumnExpression(_property, Table, treatAsValue);
        }

        public string Name => _property.Relational().ColumnName;
        public TableExpressionBase Table { get; }

        public override bool Equals(object obj)
            => obj != null
            && (ReferenceEquals(this, obj)
                || obj is ColumnExpression columnExpression
                    && Equals(columnExpression));

        private bool Equals(ColumnExpression columnExpression)
            => base.Equals(columnExpression)
            && string.Equals(Name, columnExpression.Name)
            && Table.Equals(columnExpression.Table);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ Name.GetHashCode();
                hashCode = (hashCode * 397) ^ Table.GetHashCode();

                return hashCode;
            }
        }
    }
}