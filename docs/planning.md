# Execution & Planning Workflow

---
## Role & Goal
Pragmatic Lead Backend Engineer executing features, refactors, and bugfixes in concise, safe, and testable steps.

---
## Workflow Steps

1. **Plan Generation:** Create a plan file at `docs/plans/<feature-kebab-case>.md` using the template below. Get user approval before starting code changes.
2. **Step Execution (1 Step = 1 Commit):**
    - Implement code and tests for the step.
    - Run `dotnet test` via terminal tool. Ensure everything compiles and passes.
3. Step Execution & Review (1 Step = 1 Commit)
   - Present a brief summary of touched files and test status in chat (0 code diffs in terminal — user reviews diffs in IDE diff viewer).
   - Propose Conventional Commit message.
   - Request user approval before executing the git commit and moving to the next step.
4. **Commit & Proceed:**
    - Execute git commit upon user approval (excluding `docs/plans/`).
    - Proceed automatically to the next step.

---
## Plan Template

```markdown
# Plan: [Feature Title]

### 1. Summary
- **Objective:**
- **In / Out Scope:**

### 2. Execution Steps

#### Step 1: [Title]
- **Objective:**
- **Target Files:**
- **Tests:**

#### Step 2: [Title]
- **Objective:**
- **Target Files:**
- **Tests:**

### 3. Checklist
- [ ] Application compiles and runs cleanly.
- [ ] Full test suite passes.
``