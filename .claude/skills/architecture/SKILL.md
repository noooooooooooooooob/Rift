---
name: architecture
description: Architectural decision-making framework. Requirements analysis, trade-off evaluation, ADR documentation. Use when making architecture decisions or analyzing system design.
allowed-tools: Read, Glob, Grep
---

# Architecture Decision Framework

> "Requirements drive architecture. Trade-offs inform decisions. ADRs capture rationale."

## Core Principle

**"Simplicity is the ultimate sophistication."**

- Start simple
- Add complexity ONLY when proven necessary
- You can always add patterns later
- Removing complexity is MUCH harder than adding it

## Decision Process

### 1. Understand Requirements

| Question | Purpose |
|----------|---------|
| What problem are we solving? | Core need |
| Who are the users? | Scale, patterns |
| What are the constraints? | Time, budget, team |
| What must it integrate with? | Dependencies |

### 2. Evaluate Trade-offs

Every decision has costs:

| Choice | Benefits | Costs |
|--------|----------|-------|
| Microservices | Scale, team autonomy | Complexity, latency |
| Monolith | Simple, fast | Scaling limits |
| Event-driven | Decoupling | Debugging harder |
| Sync calls | Simple to reason | Coupling, blocking |

### 3. Document Decisions (ADR)

```markdown
# ADR-001: [Decision Title]

## Status
Accepted / Proposed / Deprecated

## Context
What is the issue we're facing?

## Decision
What we decided to do.

## Consequences
What are the results, both positive and negative?
```

## Pattern Selection

### When to Use What

| Pattern | Use When | Avoid When |
|---------|----------|------------|
| Singleton | Single instance needed globally | Testing is priority |
| Observer | Many listeners, loose coupling | Simple direct calls work |
| Factory | Complex object creation | Constructor is sufficient |
| Strategy | Interchangeable algorithms | Single implementation |
| State Machine | Clear state transitions | States are ambiguous |

## Anti-Patterns

| ❌ Pattern | Problem |
|-----------|---------|
| Big Ball of Mud | No structure, everything coupled |
| Golden Hammer | Using one solution for everything |
| Premature Abstraction | Abstracting before understanding |
| Cargo Cult | Copying patterns without understanding |

## Validation Checklist

Before finalizing architecture:

- [ ] Requirements clearly understood
- [ ] Constraints identified
- [ ] Each decision has trade-off analysis
- [ ] Simpler alternatives considered
- [ ] ADRs written for significant decisions
- [ ] Team expertise matches chosen patterns

---

> The best architecture is the simplest one that meets all requirements.
