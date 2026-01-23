---
name: systematic-debugging
description: Disciplined debugging methodology. Root cause investigation before fixes. Use when troubleshooting any technical issue.
allowed-tools: Read, Glob, Grep, Bash
---

# Systematic Debugging Process

> **"NO FIXES WITHOUT ROOT CAUSE INVESTIGATION FIRST"**

Random fixes waste time and create new bugs. Quick patches mask underlying issues.

## The Four Phases

```
┌──────────────────────────────────────┐
│  Phase 1: INVESTIGATE                │
│  → Read errors, reproduce, gather    │
├──────────────────────────────────────┤
│  Phase 2: ANALYZE                    │
│  → Compare working vs broken         │
├──────────────────────────────────────┤
│  Phase 3: HYPOTHESIZE                │
│  → State theory, test ONE variable   │
├──────────────────────────────────────┤
│  Phase 4: IMPLEMENT                  │
│  → Write test, fix root cause        │
└──────────────────────────────────────┘
```

## Phase 1: Root Cause Investigation

### Steps

1. **Read error messages thoroughly**
   - Full stack trace
   - Line numbers
   - Error codes

2. **Reproduce consistently**
   - Document exact steps
   - Note when it happens
   - Note when it doesn't

3. **Review recent changes**
   - Git diff/log
   - What changed?
   - When did it start?

4. **Gather evidence**
   - Logs
   - Screenshots
   - State dumps

### Multi-Layer Debugging

| Layer | Instrument |
|-------|------------|
| Input | Log incoming data |
| Processing | Log intermediate state |
| Output | Log final result |
| Storage | Log read/write |

## Phase 2: Pattern Analysis

1. **Find working example**
   - Similar code that works
   - Previous working version
   - Documentation example

2. **Compare systematically**
   - Line by line if needed
   - Note ALL differences
   - Don't assume anything

## Phase 3: Hypothesis Testing

### Scientific Method

```
1. State specific hypothesis
   "The error occurs because X"

2. Design minimal test
   Change ONE variable only

3. Predict outcome
   "If I'm right, Y will happen"

4. Test and observe
   Did Y happen?

5. Conclude
   → Yes: proceed to fix
   → No: new hypothesis
```

## Phase 4: Implementation

1. **Write failing test first**
   - Captures the bug
   - Prevents regression

2. **Fix root cause only**
   - Single, focused change
   - No drive-by fixes

3. **Verify fix works**
   - Test passes
   - Reproduce attempt fails

## Escalation Trigger

**⚠️ If 3+ independent fixes each reveal new problems:**

This signals **architectural issue**, not isolated bugs.

→ STOP fixing
→ Document findings
→ Discuss architecture

## Red Flags

| Warning Sign | Problem |
|--------------|---------|
| "Just try changing X" | No hypothesis |
| Multiple changes at once | Can't isolate cause |
| "I'll investigate later" | Technical debt |
| "Works on my machine" | Environment issue |
| Fix breaks something else | Root cause missed |

## Anti-Patterns

| ❌ Don't | ✅ Do |
|----------|-------|
| Guess and check | Hypothesize and test |
| Change multiple things | One change at a time |
| Skip reproduction | Always reproduce first |
| Ignore error messages | Read them carefully |
| Patch symptoms | Fix root cause |

---

> Systematic debugging is faster than random guessing. Always.
