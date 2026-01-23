---
name: plan-writing
description: Task breakdown and planning framework. Creates actionable, verifiable plans. Use when implementing features or managing multi-step projects.
allowed-tools: Read, Glob, Grep
---

# Plan Writing Framework

> "A good plan is a list of tasks you can verify."

## Core Principles

### 1. Small Tasks
- **2-5 minutes each**
- One clear outcome
- Independently verifiable

### 2. Specific Actions
| ❌ Vague | ✅ Specific |
|----------|-------------|
| "Set up project" | "Run `npm init -y`" |
| "Add styling" | "Create `styles.css` with reset" |
| "Test it" | "Run `npm test` - all pass" |

### 3. Clear Verification
Every task needs a "Done when":
- Command to run
- Output to expect
- State to observe

## Plan Structure

```markdown
# Goal
One sentence: what we're building

# Tasks
1. [ ] Task description
   - Done when: [verification]

2. [ ] Task description
   - Done when: [verification]

# Done When
- [ ] Success criteria 1
- [ ] Success criteria 2

# Notes
- Constraint or consideration
```

## Task Dependencies

Mark dependencies explicitly:

```markdown
1. [ ] Create database schema
2. [ ] Create API endpoints (needs #1)
3. [ ] Create UI components
4. [ ] Connect UI to API (needs #2, #3)
```

## Plan Length

| Guideline | Reason |
|-----------|--------|
| Max 10 tasks | Keeps focus |
| Max 1 page | Easy to scan |
| If longer → split | Create sub-plans |

## Verification Types

| Type | Example |
|------|---------|
| Command | `npm test` passes |
| Visual | Button appears in header |
| State | User redirected to dashboard |
| Output | Console shows "Connected" |

## Anti-Patterns

| ❌ Don't | ✅ Do |
|----------|-------|
| Copy generic templates | Write unique plans |
| Include "maybe" tasks | Only include definite work |
| Skip verification | Every task verifiable |
| Plan too far ahead | Plan next 5-10 steps |
| Estimate time | Focus on sequence |

## Example Plan

```markdown
# Goal
Add damage calculation to battle system

# Tasks
1. [ ] Read existing combat code in Method.cs
   - Done when: understand current formula structure

2. [ ] Uncomment damage calculation function
   - Done when: code compiles without errors

3. [ ] Write unit test for basic damage
   - Done when: test exists, fails (TDD red)

4. [ ] Implement damage formula
   - Done when: test passes

5. [ ] Add defense reduction calculation
   - Done when: DEF reduces damage correctly

6. [ ] Test edge cases (0 DEF, high DEF)
   - Done when: all edge tests pass

# Done When
- [ ] All damage tests pass
- [ ] Game compiles without errors
- [ ] Manual test: attack deals expected damage
```

## Tips

- **Start planning from the end** - what does "done" look like?
- **Break down uncertain tasks** - if unsure, explore first
- **Update the plan** - plans change, that's okay
- **One task in progress** - focus on current step

---

> Each plan is UNIQUE to the task. Don't copy-paste - think through each step.
