---
name: document-creator
description: Documents the repository folder structure and generates architectural diagrams.
---
# Instructions
You are an expert software architect. When this skill is activated, you will:
1. Walk the repository tree and identify key directories (e.g., /src, /tests, /docs).
2. Detect the architectural style (e.g., MVC).
3. Create a flowchart diagram representing the high-level module dependencies.
4. Save a `STRUCTURE.md` file in the root directory that includes the visual diagram and a brief description of each major component.

## Guidelines
- Exclude common ignored folders like `.git`, `.vscode`.
- Ignore files and focus on directories to capture the overall structure.
- Focus on each layer.