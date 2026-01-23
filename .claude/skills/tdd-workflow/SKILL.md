---
name: tdd-workflow
description: Test-Driven Development workflow. RED-GREEN-REFACTOR cycle. Use when implementing new features or fixing bugs.
allowed-tools: Read, Write, Edit, Glob, Grep, Bash
---

# TDD Workflow

> Write tests first, code second.

## The TDD Cycle

```
🔴 RED     → Write failing test
    ↓
🟢 GREEN   → Write minimal code to pass
    ↓
🔵 REFACTOR → Improve code quality
    ↓
   Repeat...
```

## The Three Laws

1. Write production code **only** to make a failing test pass
2. Write **only enough** test to demonstrate failure
3. Write **only enough** code to make the test pass

## RED Phase

### What to Test

| Focus | Example |
|-------|---------|
| Behavior | "should add two numbers" |
| Edge cases | "should handle empty input" |
| Error states | "should throw for invalid data" |

### Rules

- Test MUST fail first
- Test name describes expected behavior
- One assertion per test (ideally)

## GREEN Phase

### Minimum Code

| Principle | Meaning |
|-----------|---------|
| **YAGNI** | You Aren't Gonna Need It |
| **Simplest thing** | Write the minimum to pass |
| **No optimization** | Just make it work |

### Rules

- Don't write unneeded code
- Don't optimize yet
- Pass the test, nothing more

## REFACTOR Phase

### What to Improve

| Area | Action |
|------|--------|
| Duplication | Extract common code |
| Naming | Make intent clear |
| Structure | Improve organization |
| Complexity | Simplify logic |

### Rules

- All tests must stay green
- Small incremental changes
- Commit after each refactor

## AAA Pattern

Every test follows:

```
ARRANGE → Set up test data
ACT     → Execute code under test
ASSERT  → Verify expected outcome
```

## When to Use TDD

| Scenario | TDD Value |
|----------|-----------|
| New feature | High |
| Bug fix | High (test first) |
| Complex logic | High |
| Exploratory | Low (spike first) |
| UI layout | Low |

## Test Prioritization

| Priority | Test Type |
|----------|-----------|
| 1 | Happy path |
| 2 | Error cases |
| 3 | Edge cases |
| 4 | Performance |

## Anti-Patterns

| ❌ Don't | ✅ Do |
|----------|-------|
| Skip RED phase | Watch test fail first |
| Write tests after | Write tests before |
| Over-engineer initial | Keep it simple |
| Multiple asserts | One behavior per test |
| Test implementation | Test behavior |

## Unity Testing

### Edit Mode Tests
- Fast, no scene required
- Pure logic testing
- Use for: calculations, data validation

### Play Mode Tests
- Requires scene
- Tests MonoBehaviour lifecycle
- Use for: integration, game flow

```csharp
[Test]
public void Damage_ReducesHealth()
{
    // Arrange
    var unit = new Unit { Health = 100 };

    // Act
    unit.TakeDamage(30);

    // Assert
    Assert.AreEqual(70, unit.Health);
}
```

---

> The test is the specification. If you can't write a test, you don't understand the requirement.
