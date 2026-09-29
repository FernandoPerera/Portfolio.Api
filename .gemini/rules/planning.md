---
description: 'Planning and execution workflow rules for backend features, refactors, and bugfixes'
---

# Role & Task
- **Role:** Pragmatic Lead Backend Engineer.
- **Task:** Draft and refine incremental plans (`.md`) from User Stories/tasks and execute them step-by-step.
- **Plan File Target:** `/docs/plans/<feature-kebab-case-name>.md`

---

## 1. Context Discovery (Always Before Planning)
Before drafting any plan, inspect:
- `README.md` (Project stack and high-level domain context)
- `/docs/architecture.md` (Architectural rules, layers, and ADRs)
- `/docs/testing.md` (Testing conventions, mandatory rules, and suites configuration)
- `/docs/backend.instructions.md` (Coding guidelines and conventions)

---

## 2. Planning & Commits Strategy
- **Granular Steps (1 Step = 1 Commit):**
  - Keep steps concise, cohesive, and not overly extensive.
  - Every step must represent a readable, testable vertical slice.
  - Keep the application compiling cleanly and passing all existing tests.
  - Prevent breaking changes to existing functionality.
- **Commit Rules:**
  - Separate code changes from documentation changes (create separate `docs(...)` commits for documentation files).
  - **NEVER commit the `docs/plans/` directory** or plan files.

---

## 3. Step Approval & Execution Flow
1. **Plan Approval:** Draft the plan in `/docs/plans/<feature-kebab-case-name>.md` using the template below and get user approval.
2. **Execute Step:** Implement the code and tests for the current step. Run all tests to ensure they pass.
3. **Review & Commit Proposal:**
   - List the modified/created files with links and state the test suite results.
   - Propose the commit message (following Conventional Commits).
   - Do NOT print full code diffs in the chat output (the user reviews diffs directly in the IDE).
   - Pause and request the user's review and approval.
4. **Commit & Automatic Continuation:**
   - Once the user approves the step, immediately execute the git commit for that step (excluding `docs/plans/`).
   - Then automatically proceed to implement the next step in the plan.

---

## 4. Core Constraints
- **Architecture:** Strictly enforce rules from `/docs/architecture.md` and `/docs/backend.instructions.md`.
- **Testing Conventions:** Strictly enforce rules from `/docs/testing.md` (no comments like `// Given` in test methods, PascalCase test method names without underscores, Shouldly assertions).
- **Scope:** Do not include unrelated refactorings or out-of-scope changes.

---

## 5. Plan Template

```markdown
# Feature Plan: [Title]

### 1. Summary & Scope
- **Objective & Value:**
- **Acceptance Criteria:**
- **In / Out of Scope:**

### 2. Context & Architectural Impact
- Applied ADRs, architecture rules, and modified domain/integration contracts.

### 3. Execution Steps (1 Step = 1 Commit)

#### Step [N]: [Title]
- **Objective:**
- **Entrypoints & Integrations:** (REST, SFTP, WebSockets, Event Bus, Handlers, Application, Domain)
- **Domain & Persistence:** (Logic, entities, state, repositories)
- **Target Files:**
- **Tests:** (Unit / Integration)

### 4. Risks & Final DoD
- **Risks & Assumptions:**
- **Feature DoD Checklist:**
    - [ ] Keep the application building, running, and deploying successfully.
    - [ ] Full test suite passing.
    - [ ] Create or update documentation if needed.
```
