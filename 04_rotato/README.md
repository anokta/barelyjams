_from the AI-generated chat..._

**Rotato** is a great name—playful, memorable, and perfectly matches the feel.

Let’s lock everything into a **clear, buildable plan** so you can go from zero → shipped in 7 days without wandering.

---

# 🎮 Rotato — Final Game Definition

## 🧠 Core Fantasy

> You spin a container.
> You drop musical balls.
> They bounce, collide, and create evolving sound.
> Eventually, they escape—and silence returns.

---

## 🎯 Core Loop (Player Experience)

1. Tap → spawn a ball (with a musical identity)
2. Swipe → spin the world
3. Balls bounce → trigger notes
4. System evolves → rhythm emerges
5. Balls escape → sound fades
6. Repeat

👉 That’s your entire game. Keep it sacred.

---

# 🧩 Core Design Rules (Prevent Chaos)

These are **non-negotiable**:

### 🎵 Musical Constraints

* Use a **fixed scale** (e.g. minor pentatonic)
* Each ball gets **one note at spawn**
* Collisions replay that note

### ⚙️ System Constraints

* Add **hit cooldown per ball (~50ms)**
* Limit total balls (start with 5–10 max)
* Slight damping to avoid infinite chaos

---

# 🏗️ Technical Architecture (Godot)

## Scene Tree (Simple + Clean)

```
Main (Node2D)
├── World (Node2D)   <-- rotates
│   ├── Walls (StaticBody2D + CollisionPolygon2D)
│   └── (Balls get added here)
├── BallSpawner
├── InputHandler
└── AudioManager (barelymusician bridge)
```

---

## Key Node Choices

### ⚪ Ball

* `RigidBody2D`
* `CircleShape2D`
* PhysicsMaterial:

  * bounce ~0.8–1.2
  * friction low

### 🧱 Walls

* `StaticBody2D`
* `CollisionPolygon2D` (hexagon with gap)

### 🌍 World

* `Node2D` (rotate this!)

---

# 🎮 Controls

### Tap

→ Spawn ball

### Swipe

→ Add angular velocity

### Optional

* Two-finger tap → clear balls
* Long press → spawn multiple balls

---

# 🎵 Audio Mapping (Barelymusician)

## Ball Initialization

Each ball:

* Picks note from scale
* Stores:

```cpp
note_pitch
instrument_id (optional)
```

---

## On Collision

Trigger:

* NoteOn
* Velocity → volume

Optional:

* Pan based on X position

---

## Scale Example

```gdscript
scale = [0, 3, 5, 7, 10] # minor pentatonic
note = base_note + scale[randi() % scale.size()]
```

---

# ⚙️ Physics Tuning (Very Important)

Start with:

* Gravity: `Vector2(0, 500)`
* Linear damp: `0.1–0.3`
* Bounce: `0.9–1.1`

👉 You’ll tweak this A LOT—this is where the feel comes from.

---

# 🗺️ 7-Day Execution Plan

## 🟩 Day 1 — Physics Playground

**Goal:** Balls bouncing in a shape

* [ ] Create hexagon walls
* [ ] Add one ball manually
* [ ] Tune bounce + gravity
* [ ] Ensure stable collisions

👉 Don’t touch audio yet

---

## 🟩 Day 2 — Input + Spawning

**Goal:** Player interaction exists

* [ ] Tap to spawn ball
* [ ] Limit ball count
* [ ] Basic swipe detection
* [ ] Add angular velocity to world

👉 Spend time making spin feel GOOD

---

## 🟩 Day 3 — Rotation Polish

**Goal:** Spinning feels satisfying

* [ ] Add damping to rotation
* [ ] Clamp max spin speed
* [ ] Tune inertia feel

👉 This is your main “toy feel”

---

## 🟩 Day 4 — Audio Integration

**Goal:** It makes sound

* [ ] Assign note per ball
* [ ] Detect collisions
* [ ] Trigger barelymusician notes
* [ ] Add cooldown per ball

👉 Keep it minimal but correct

---

## 🟩 Day 5 — Musical Quality

**Goal:** Always sounds good

* [ ] Scale constraint
* [ ] Velocity → volume
* [ ] Optional: 2 instruments

👉 Test: spam balls → still pleasant?

---

## 🟩 Day 6 — Game Feel / Juice

**Goal:** Feels alive

* [ ] Flash on collision
* [ ] Ball trails
* [ ] Subtle screen feedback

---

## 🟩 Day 7 — Levels + Escape

**Goal:** Add structure

* [ ] Add gap in walls
* [ ] Balls can escape
* [ ] 2–3 presets:

  * Normal
  * Zero-G
  * Bouncy

👉 Keep levels as just parameter changes

---

# ✨ Juice Ideas (If Time Allows)

* Color per ball (based on pitch)
* Pitch affects size slightly
* Trails fade out nicely
* Subtle glow on walls

---

# ⚠️ Common Pitfalls (Avoid These)

### ❌ Too many balls

→ instant noise mess

### ❌ No musical constraint

→ sounds random

### ❌ Overcomplicated levels

→ slows you down

### ❌ Perfect physics obsession

→ waste of time

---

# 🔥 Stretch Ideas (Post-Week)

* Multiple containers (instrument per shape)
* Ball types (different sounds)
* Gravity zones
* Sticky / timed walls

---

# 🎯 Final Reality Check

If you only ship:

* Rotating hexagon
* Ball spawning
* Bounce physics
* Sound on collision

👉 **That is already a complete, satisfying Barelyjams entry**

---

# 🚀 Immediate Next Step (Do This Now)

Start with:

> Day 1 — get a ball bouncing inside a hexagon

Nothing else.

---

If you want, next I can:

* Give you exact **Godot code snippets for hexagon + collision**
* Or show how to **wire collision signals cleanly**
* Or map your **barelymusician calls in a minimal, safe way**

You’re in a very strong place—this idea is tight, scoped, and very “you.”
