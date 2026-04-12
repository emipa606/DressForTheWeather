# Copilot Instructions for Dress for the Weather (Continued)

## Mod Overview and Purpose

Dress for the Weather (Continued) is a RimWorld mod designed to enhance the gameplay experience by ensuring that non-player characters (NPCs) such as trade caravans and guests arrive on the map dressed appropriately for the current environmental conditions. Originally created by Daniel, this continued version updates and refines its features.

## Key Features and Systems

- **Weather-Appropriate Apparel:** NPCs will adapt their clothing to suit the current temperatures. In cold climates, parkas are favored, while in hot climates, dusters and cowboy hats are common.
- **Hazard Protection:** In areas with toxic fallout or high pollution, NPCs from factions with industrial-level technology or higher will equip gas masks.
- **Faction-Specific Apparel:** NPCs choose apparel based on their tech level, ensuring an immersive gameplay experience.
- **Adjustable Settings:** Players have control over what kind of apparel guests are willing to wear, with options to disable armor and certain utilities.
- **Compatibility:** This mod is designed to work seamlessly with Combat Extended (CE) and can be added or removed mid-save without issues.

## Coding Patterns and Conventions

- **Class Design:** Utilize static classes for utility methods that do not require instance fields, such as `IncidentWorker_NeutralGroup_SpawnPawns` and `PawnsArriveUtilities`.
- **Mod Integration:** Utilize derived classes like `DressForTheWeatherMod` and `DressForTheWeatherSettings` to integrate settings into RimWorld’s mod framework.
- **Performance Optimization:** Consider performance impacts during NPC spawn events and optimize code to minimize lag.

## XML Integration

- **Defining Apparel:** Use XML files to define apparel items and their attributes so they can be dynamically applied based on environmental factors.
- **Tech Level Restrictions:** Ensure apparel XML files respect faction tech level restrictions, potentially leveraging tags or labels.

## Harmony Patching

- **Core Function Overloads:** Utilize Harmony to apply patches on game methods that handle NPC spawning. This ensures that newly spawned characters check weather conditions for apparel adjustments without altering vanilla game code.
- **Adding Patches:** Target methods related to guest and raider spawn such as `IncidentWorker_NeutralGroup_SpawnPawns` and add appropriate prefix/postfix methods.

## Suggestions for Copilot

- **Predictive Code Suggestions:** Implement code suggestions that respect existing conventions, such as using `foreach` loops for iterating over collections.
- **Integration Assistance:** Propose suggested Harmony patches by identifying RimWorld methods likely to benefit from apparel checks.
- **XML Suggestions:** Provide templates for new apparel def XML files, including necessary tags important for this mod’s features.
- **Debugging and Testing:** Suggest tools and methods for testing new patches or configurations, such as using snippets that log NPC spawn events.
- **Refactoring Suggestions:** Recommend organizational improvements to reduce complexity in files handling settings or multiple features.

By following these instructions, developers and contributors can maintain and expand upon the Dress for the Weather mod in a consistent and efficient manner, ensuring that both the codebase and player experience remain well-optimized and enjoyable.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.
