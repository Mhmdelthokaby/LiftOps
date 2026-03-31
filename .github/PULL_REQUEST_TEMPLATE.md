## Tenant Safety Checklist

- [ ] New/updated tenant-scoped entities include `CompanyId`.
- [ ] New/updated tenant-scoped entities are covered by global tenant query filters.
- [ ] No unfiltered raw SQL or `Set<T>()` access bypasses tenant constraints.
- [ ] Any `ListAllAsync()` usage is safe under tenant query filters or replaced with scoped querying.

## Validation

- [ ] Ran relevant tests locally.
