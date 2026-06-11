# GitHub Copilot Instructions for Dress For The Weather Mod

## Mod Overview and Purpose

Dress for the Weather (Continued) is a mod designed to enhance the realism and immersion of RimWorld by ensuring that visiting trade caravans, guests, and optionally raiders dress appropriately for the current weather conditions. Originally made by Daniel and updated for continued compatibility, this mod aims to prevent issues such as hypothermia and frostbite in cold climates and overheating in hot ones, providing a more sensible behavior for AI characters upon entering your map.

## Key Features and Systems

- **Weather-Appropriate Apparel:** Upon spawn, guests' and trade caravan members' apparel is adjusted based on the current outdoor temperature. Cold weather prompts parkas, while hot weather calls for dusters and cowboy hats.
- **Pollution and Toxic Fallout Protection:** Pawns from industrial-tech level factions will wear gas masks on polluted or toxic maps.
- **Faction-Specific Tech Level Apparel:** Guests and raiders (if enabled) will wear clothing that aligns with their tech level.
- **Configurable Apparel Settings:** Players can adjust settings to toggle what apparel types are preferable for guests, such as disabling certain armors and utility items.
- **Compatibility and Performance:** Compatible with Combat Extended, with a minimal performance impact confined to spawn times.

## Coding Patterns and Conventions

- The project follows typical C# project organization with source files categorized by their functionality.
- The naming conventions use PascalCase for classes and methods, ensuring readability and maintainability.
- Consistent use of comments to describe class and method purposes helps with code navigation and understanding.

## XML Integration

- XML files are present to handle mod metadata (`About.xml`) and settings configurations (`Manifest.xml`).
- XML structure follows RimWorld's standard for defining mods and patches.

## Harmony Patching

- The mod uses Harmony for patching to enhance and alter the default game behavior:
  - **IncidentWorker_NeutralGroup_SpawnPawns.cs**: Contains patches for modifying pawn apparel on spawn.
  - **IncidentWorker_Raid_TryGenerateRaidInfo.cs**: Handles settings for raider apparel replacement.

## Suggestions for Copilot

- **Context Awareness**: Use comments and existing method names in C# files to maintain the logic flow while generating code for apparel swapping and condition checks.
- **XML Integration**: Suggest patterns for generating new XML settings or tweaks easily, using existing files as templates.
- **Harmony Patches**: Ensure that Harmony patches are suggested in line with the existing postfix conventions, and maintain compatibility with other mods by non-invasive patching techniques.
- **Performance Considerations**: Whenever writing new C# methods, prioritize suggestions that minimize performance impact, especially during the spawn of new pawns.
- **Testing and Debugging**: Suggest integration points for logging and managing mod compatibility issues, which can assist in debugging when new Copilot-suggested features are tested.

This documentation provides essential insights into the "Dress for the Weather (Continued)" mod and guidelines to enhance development with the aid of GitHub Copilot.


This .github/copilot-instructions.md file is a comprehensive guide designed to help developers working on the "Dress for the Weather (Continued)" mod. It encapsulates critical information about the mod, aiding both understanding and future development.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).

