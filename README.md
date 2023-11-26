# Descant

*dess • can’t*

> 1. an independent treble melody sung or played above a basic melody.
> 2. a melodious song.
> 3. a discourse on a theme.



***Disclaimer: Currently, `.desc.json` and `.descactor.json` files can only be opened from the Assets directory, not the Packages directory. If you want to open the test files in the `Examples` folder, simply copy them to anywhere in the Assets folder.***



## Overview

**Descant** is an in-development **Unity** dialogue system plugin. The [Unity Asset Store](https://assetstore.unity.com) is [chock full](Documentation/system_review.xlsx) of many such types of plugins, ranging from [feature-rich](https://assetstore.unity.com/packages/tools/behavior-ai/dialogue-system-for-unity-11672), to [ultra-minimalist](https://assetstore.unity.com/packages/tools/visual-scripting/conversa-dialogue-system-192549), to [downright bad](https://assetstore.unity.com/packages/tools/c5-dialogue-system-14881). **Descant** aims to hit the sweet spot between quality UI, powerful features, and easy-to-lean functionality, while also addressing many of the game-specific consequences of the standard dialogue manager setup. Besides acting as a standard tool for creating, saving, and actualizing non-linear game dialogue, it also pushes the enveloppe by adding optional ‘dialogue enhancements’ that introduce features to break away from the overused and underwhelming trends seen in many interactive fiction games. These enhancements act similar to **Unity**’s standard `GameObject` `Component` system, and can be applied at-will to nodes, characters within the conversation, and the conversation itself. This modular approach is so-far not explored in the world of Unity dialogue systems. The project will be free *(and hopefully collaborative open-source)* forever. Feel free to send me a message or submit a pull request if you want to make any changes.



## Installation

1. Install the latest release from the [GitHub](https://github.com/Owmacohe/Descant/releases) repository.
2. Within your Unity project, navigate to the **Package Manager** window.
3. Within the **Package Manager**, click on the **+** icon in the top left, and select **Add package from disk…**
4. Navigate toe the downloaded **Descant** folder, and within that folder, select `package.json`.
5. A sample scene can be found at: `Packages/Descant/Examples/Test_Scene.unity`.
6. Opening this scene may prompt you to install **Text Mesh Pro**. Simple click on **Import TMP Essentials** to do so.



## Usage

1. The **Descant Editor** can be opened with `Tools/Descant/Descant Editor`.
2. **Descant Graphs** can be created by right clicking, and selecting `Create/Descant Graph`.
3. **Descant Graphs** can be edited by right clicking on a `.desc.json` file, and selecting `Edit Descant Graph`.
4. ***A formal tutorial for editor and runtime usage is coming soon!***



## Documentation

- This `README`
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
