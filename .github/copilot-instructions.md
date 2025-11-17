# GitHub Copilot Instructions for "Dress for the Weather (Continued)"

## Mod Overview and Purpose
**"Dress for the Weather (Continued)"** enhances the realism and playability of RimWorld by ensuring that visiting trade caravans, guests, and optionally raiders, wear climate-appropriate apparel based on current environmental conditions. This prevents issues such as hypothermia and frostbite among visitors who would otherwise dress inappropriately for the weather, thus improving immersion and reducing the need for player intervention.

## Key Features and Systems
- **Weather-Aware Apparel**: Guests and caravans spawn with clothing suited to the map's temperature, preventing adverse effects due to inappropriate apparel.
- **Tech Level Integration**: Apparel choices respect each faction's technological limits, ensuring a logical progression of items worn.
- **Polluted and Toxic Weather Adaptation**: On polluted maps or during toxic fallout, industrial-level factions wear gas masks for added protection.
- **Extended Functionality**: Optional setting allows raiders to also dress according to weather conditions.
- **Flexible Settings**: The mod allows customization of which items are considered for wear, preventing excessive or overpowered apparel use.
- **Compatibility and Performance**: Safe to add or remove mid-save; has minimal performance impact outside of spawning events.

## Coding Patterns and Conventions
- **Class Naming**: Classes utilize PascalCase, such as `DressForTheWeatherMod` and `DressForTheWeatherSettings`.
- **Method organization**: Separation of concerns is maintained with code divided across multiple files, like `DressForTheWeatherMod.cs` for main mod class and settings, and `IncidentWorker_PawnsArrivePatch.cs` for patching logic.
- **Modular Methods**: Methods are designed to be modular and reusable, focusing on specific tasks related to apparel and spawning logic.

## XML Integration
The mod integrates with XML files to define apparel choices and settings:
- XML definitions control the apparel options available to pawns, ensuring that faction limits and player settings are respected.
- XML data allows dynamic changes based on JSON-defined mod settings, making the apparel selection adaptive to game conditions.

## Harmony Patching
- **Patch Class**: `IncidentWorker_PawnsArrivePatch` is used to inject code at specific points of RimWorld's incident worker logic.
- **Patching Techniques**: Harmony's prefix and postfix methods are utilized to seamlessly integrate mod features without altering core game code.
- **Conflict Resolution**: Harmony patches are designed to minimize conflicts with other mods and preserve game stability.

## Suggestions for Copilot
1. **Consistent Naming**: Ensure classes, methods, and variables are consistently named using PascalCase for classes and camelCase for variables.
2. **Documentation**: Copilot should suggest inline comments for complex logic within methods to maintain readability.
3. **Error Handling**: Encourage error logging methods to catch and report errors, aiding in debugging.
4. **Performance**: Suggest code optimizations to reduce performance impact during the pawn spawning process.
5. **Mod Compatibility**: Generate suggestions for checking compatibility with popular mods, ensuring seamless integration.
6. **Extendability**: Offer suggestions for future functionality, such as automatic adaptation to newly added apparel from other mods.

By following these instructions, you can effectively leverage GitHub Copilot to enhance and debug the "Dress for the Weather (Continued)" mod, ensuring it remains an enjoyable and immersive addition to RimWorld.
