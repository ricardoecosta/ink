# Hamstas'n'Kitties Port - Changelog

All changes for the MonoGame modernization port from legacy Xamarin to modern .NET 8 / MonoGame.

---

## [Unreleased]

### Phase 0: Project Setup
*Started: 2026-02-14*
*Completed: 2026-02-14*

#### Added
- Initial CHANGELOG.md for tracking all port changes
- New SDK-style .NET 8 solution structure (`HamstasKitties.sln`)
- `src/HamstasKitties.Shared/` - Shared game library project
- `src/HamstasKitties.Desktop/` - Desktop MonoGame project with Content pipeline
- `tests/HamstasKitties.Shared.Tests/` - xUnit test project with FluentAssertions
- `.github/workflows/build.yml` - CI/CD pipeline (build + test + lint)
- `Directory.Build.props` - Common build settings (warnings as errors, code style)
- `.editorconfig` - Editor configuration for consistent formatting

#### Changed
- Renamed legacy `HamstasKitties.sln` to `HamstasKitties.Legacy.sln`

---

## Change Log Format

Each change entry follows this format:

```
#### YYYY-MM-DD - Brief Description
- **Phase:** X
- **Files:** path/to/file1.cs, path/to/file2.cs
- **Description:** Detailed description of the change
- **Breaking Changes:** None | Description of breaking changes
```

---

## Phase Progress

| Phase | Status | Start Date | End Date |
|-------|--------|------------|----------|
| 0: Project Setup | Complete | 2026-02-14 | 2026-02-14 |
| 1: Core Abstractions | Not Started | - | - |
| 2: Shared Code Migration | Not Started | - | - |
| 3: UI Framework Migration | Not Started | - | - |
| 4: Platform Projects | Not Started | - | - |
| 5: Firebase Integration | Not Started | - | - |
| 6: Assets Migration | Not Started | - | - |
| 7: Testing & QA | Not Started | - | - |
| 8: Polish & Deploy | Not Started | - | - |

---

## Technical Debt Resolved

| Issue | Location | Resolution | Date |
|-------|----------|------------|------|
| - | - | - | - |

---

## Deprecations Removed

| Service/Library | Replaced By | Date |
|-----------------|-------------|------|
| Scoreloop | Firebase Firestore | - |
| Flurry Analytics | Firebase Analytics | - |
| Windows Phone conditionals | Removed | - |

---

## Notes

- All dates in YYYY-MM-DD format
- Breaking changes require major version bump
- Each phase completion requires code review sign-off
