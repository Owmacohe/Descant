# Descant

*dess • can’t*

> 1. an independent treble melody sung or played above a basic melody.
> 2. a melodious song.
> 3. a discourse on a theme.



***Disclaimer: `.desc.json` and `.descactor.json` files can only be opened from the Assets directory, not the Packages directory. If you want to open the test files in the `Examples` folder, simply copy them to anywhere within the Assets folder.***



## Overview

**Descant** is a **Unity** dialogue system plugin. The [Unity Asset Store](https://assetstore.unity.com) is [chock full](Documentation/system_review.xlsx) of many such types of plugins, ranging from [feature-rich](https://assetstore.unity.com/packages/tools/behavior-ai/dialogue-system-for-unity-11672), to [ultra-minimalist](https://assetstore.unity.com/packages/tools/visual-scripting/conversa-dialogue-system-192549), to [downright bad](https://assetstore.unity.com/packages/tools/c5-dialogue-system-14881). **Descant** aims to hit the sweet spot between quality UI, powerful features, and easy-to-lean functionality, while also addressing many of the game-specific consequences of the standard dialogue manager setup. Besides acting as a standard tool for creating, saving, and actualizing non-linear game dialogue, it also pushes the envelope by adding optional dialogue-enhancing node components that introduce features to break away from the overused and underwhelming trends seen in many interactive fiction games. These enhancements act similar to **Unity**’s standard `GameObject` `Component` system, and can be applied at-will to nodes. This modular approach is so-far not explored in the world of Unity dialogue systems. The project will be free *(and collaborative open-source)* forever. Feel free to send me a message or submit a pull request if you want to make any changes.



## Installation

1. Install the latest release from the [GitHub repository](https://github.com/Owmacohe/Descant/releases), unzip it, and place the folder into your Unity project's `Packages` folder
2. Within your Unity project, navigate to the **Package Manager** window
3. Within the **Package Manager**, click on the **+** icon in the top left, and select **Add package from disk…**
4. Navigate to the zipped **Descant** folder, and within that folder, select `package.json`
5. A sample scene can be found at: `Examples/Test_Scene.unity`
6. Opening this scene may prompt you to install **Text Mesh Pro**. Simple click on **Import TMP Essentials** to do so



## Descant Files

- **Descant Graphs**
  - **Descant Graphs** can be created by right clicking, and selecting `Create/Descant Graph`
  - **Descant Graphs** can be edited by right clicking on a `.desc.json` file, and selecting `Edit Descant Graph`
- **Descant Actors**
  - **Descant Actors** can be created by right clicking, and selecting `Create/Descant Actor`
  - **Descant Actors** can be edited by right clicking on a `.descactor.json` file, and selecting `Edit Descant Actor`



## Usage
- **Descant Graphs**
  1. The **Descant Graph Editor** can be opened with `Tools/Descant/Descant Graph Editor` or by creating/editing a **Descant Graph** file
  1. Use middle-click to pan around in the editor
  1. New nodes can be created by right-clicking within the grid
  1. Connections between nodes can be created by left-clicking on nodes’ ports, and dragging to create a connection line to another port
  1. `ChoiceNode`s represent player choices within a given dialogue, and `ResponseNode`s represent the NPC’s responses or statements. To inject a DescantActor’s statistic into the text, write `{actor_name:statistic_name}`
  1. The `StartNode` represents the place where a given dialogue begins, and `EndNode`s represent where it can end
  1. More complex functionality can be added to nodes by adding `Components` (see the [Descant documentation](https://omch.tech/descant) for more info on each default component, as well as how to write your own)
- **Descant Actors**
  1. The **Descant Actor Editor** can be opened with `Tools/Descant/Descant Actor Editor` or by creating/editing a **Descant Actor** file
  2. New **Statistics**, **Topics**, and **Relationships** can be added with their respective **Add** buttons
     - **Statistics** are variables that pertain to actors such as health, level, stamina, etc.
     - **Topics** are names of characters, locations, events, concepts, etc. that the actors may learn
     - **Relationships** are values quantifiably that represent how actors feel about each other, concepts, factions, etc.
  3. The **Dialogue attempts** represents the number of times that this actor has been talked to by the player
- **Runtime**
  1. Drag the `ConversationUI` prefab into your Unity scene (you may have as many `ConversationUI`s as you want in the same scene, and you may modify their text and UI styles as much as you want, so long as the `DescantConversationUI`'s inspector assignments don’t get broken)
  2. Add an `Event System` object to your scene, if you don't already have one
  3. Add a `DescantDialogueTrigger` script to a GameObject of your choice, and assign its fields (hover over each field to see a popup of its description) (you may have as many `DescantDialogueTrigger`s as you want in the same scene)
  4. At some point while the game is running, call the `Display()` method in the `DescantDialogueTrigger` script to begin the dialogue *(e.g. when the player presses `[E]`, or when a `Button` is clicked)*



## Documentation

- This `README`
- [Descant documentation](https://omch.tech/descant)
- Planning
  - [Initial research](Documentation/interaction_research.md), [market survey](Documentation/system_review.xlsx), and [key pitfalls and successes](Documentation/pitfalls_and_sucesses.md)
  - [Features list](Documentation/features.md)
  - [Weekly process journal](Documentation/journal.md)

- [Unity Asset Store page]() *(will be created near the end of the project)*



## Inspiration/Sources

- [BlabScript](https://www.lablablab.net/?p=701)
- [Ultima Ratio Regum](https://www.markrjohnsongames.com/games/ultima-ratio-regum)
- [Emily Short's Interactive Storytelling](https://emshort.blog/how-to-play/writing-if/my-articles/conversation)
- [A Gossip Virtual Social Network for Non Playable Characters in Role Play Games](https://ieeexplore.ieee.org/document/6680108?part=1)
- [NiEngine](https://github.com/StephanieRct/NiEngine)
