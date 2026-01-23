---
name: game-development
description: Game development orchestrator. Routes to platform-specific sub-skills. Use for any game project - handles architecture, patterns, performance.
allowed-tools: Read, Write, Edit, Glob, Grep, Bash
---

# Game Development Orchestrator

> "Great games come from iteration, not perfection. Prototype fast, then polish."

## The Universal Game Loop

```
┌─────────────────────────────────────────┐
│              GAME LOOP                  │
├─────────────────────────────────────────┤
│  INPUT    → Read player actions         │
│  UPDATE   → Process game logic          │
│            (fixed timestep)             │
│  RENDER   → Draw the frame              │
│            (interpolated)               │
├─────────────────────────────────────────┤
│  Target: 60 FPS = 16.67ms per frame     │
└─────────────────────────────────────────┘
```

## Performance Budget (60 FPS)

| System | Budget | Notes |
|--------|--------|-------|
| Input | 1ms | Poll & queue |
| Physics | 3ms | Fixed timestep |
| AI | 2ms | Time-sliced |
| Game Logic | 4ms | Update entities |
| Rendering | 5ms | Draw calls |
| **Buffer** | 1.67ms | Headroom |

## Core Patterns

### State Machine (Simple)
```
Use for: Player states, UI screens, game phases
When: < 10 states, clear transitions
```

### Object Pool
```
Use for: Bullets, particles, enemies
When: Frequent spawn/destroy cycles
Why: Avoid GC spikes
```

### Entity Component System (ECS)
```
Use for: Large entity counts (1000+)
When: Performance critical
Why: Cache-friendly iteration
```

### Behavior Trees
```
Use for: Complex AI decisions
When: Multiple condition branches
Why: Modular, debuggable
```

## Decision Matrix

### By Dimension

| Type | Focus Areas |
|------|-------------|
| **2D** | Sprites, tilemaps, 2D physics, pixel-perfect |
| **3D** | Meshes, shaders, 3D physics, LOD |

### By Platform

| Platform | Constraints |
|----------|-------------|
| **PC** | Memory generous, input variety |
| **Mobile** | Touch input, battery, thermal |
| **Web** | Download size, browser limits |
| **VR/AR** | 90fps minimum, comfort |

## Anti-Patterns

| ❌ Don't | ✅ Do |
|----------|-------|
| Update everything every frame | Use events, dirty flags |
| Create objects in hot loops | Pool reusable objects |
| Deep inheritance trees | Prefer composition |
| Hardcode game constants | Use ScriptableObjects/config |
| Premature optimization | Profile first, then optimize |

## Unity-Specific Guidelines

### Architecture
- Use **ScriptableObjects** for data (stats, configs)
- Use **Singletons** sparingly (Manager pattern)
- Prefer **composition** over inheritance
- Use **interfaces** for decoupling

### Performance
- Cache GetComponent results
- Use object pooling for spawned objects
- Avoid Find() in Update()
- Use events over polling

### Project Structure
```
Assets/
├── Scripts/
│   ├── Core/         # Managers, systems
│   ├── Entities/     # Units, characters
│   ├── UI/           # UI components
│   └── Utils/        # Helpers
├── Prefabs/
├── ScriptableObjects/
├── Resources/
└── Scenes/
```

## Validation Checklist

Before shipping:
- [ ] Frame time stable (no spikes)
- [ ] Memory usage flat (no leaks)
- [ ] Input feels responsive
- [ ] Load times acceptable
- [ ] No null reference exceptions

---

> Start simple. Add complexity only when needed. Profile before optimizing.
