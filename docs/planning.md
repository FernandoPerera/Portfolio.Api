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
3. **Commit & Review (1 Step = 1 Commit):**
    - Once the step compiles and `dotnet test` passes, commit it right away with a Conventional Commit message (excluding `docs/plans/`).
    - Give a brief summary of touched files and test status in chat (0 code diffs in terminal).
    - The user reviews the commit manually and replies; wait for the approval.
4. **Approve or Reject:**
    - If the user approves and the step commit touched more than 10 files, split it into micro-commits grouped by context (`git reset --soft HEAD~1`, then one commit per context). Each micro-commit must compile and pass `dotnet test`.
    - Then proceed to the next step.
    - If the user rejects, run `git reset --soft HEAD~1` and discuss the problem with the user before changing anything.
5. **Push & Pull Request:**
    - When the user asks to push, push the branch and generate the PR with a title (Conventional Commit style) and a short description (Summary, Changes, Testing).
    - Use `gh pr create` when available; otherwise print the title and description for the user to paste.

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