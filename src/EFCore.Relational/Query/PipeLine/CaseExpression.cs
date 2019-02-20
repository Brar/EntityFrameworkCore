﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Relational.Query.PipeLine
{
    public class CaseExpression : SqlExpression
    {
        public CaseExpression(
            IEnumerable<CaseWhenClause> whenClauses, SqlExpression elseResult,
            Type type, RelationalTypeMapping typeMapping, bool condition)
            : base(type, typeMapping, condition)
        {
            WhenClauses = whenClauses;
            ElseResult = elseResult;
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var changed = false;
            var whenClauses = new List<CaseWhenClause>();
            foreach (var whenClause in WhenClauses)
            {
                var test = (SqlExpression)visitor.Visit(whenClause.Test);
                var result = (SqlExpression)visitor.Visit(whenClause.Result);

                if (test != whenClause.Test || result != whenClause.Result)
                {
                    changed = true;
                    whenClauses.Add(new CaseWhenClause(test, result));
                }
            }

            var elseResult = (SqlExpression)visitor.Visit(ElseResult);

            return changed || elseResult != ElseResult
                ? new CaseExpression(whenClauses, elseResult, Type, TypeMapping, IsCondition)
                : this;
        }

        public IEnumerable<CaseWhenClause> WhenClauses { get; }
        public SqlExpression ElseResult { get; }
    }
}
