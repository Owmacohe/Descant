# Dialogue System Pitfalls

[toc]



## Types of interactions

- **Yes/No:** Simple responses to simple questions
- **Talk To:** Static and simple
  - **Menu Conversations:** Talk-To + dropdown of options
- **Ask/Tell:** Binary of options to ask or tell an NPC about some keyword
  - Can also have special topics
  - Can also have a tone/stance applied
  - **Topic Words:** Ask/Tell without the verbs
  - **Chatbot:** NLP skin over Ask/Tell
- **Topics:** Classic conversation option dropdown



## Conversation Information

- **Topic:** Subject of the conversation
- **Fact:** Proposition about a topic
- **Quip:** Interjection of pre-written dialogue
- **Effect:** Result of some dialogue choice
- **Goal:** Something the player or NPC is trying to achieve
- **Scene:** Narrative ‘section’ of the conversation



## Critiques

- Sameness in choices / Repetitious dialogue
- Clicking fatigue / Ask-everyone-about-everything tedium
- Unchanging NPCs
- Adventure-style text-parsers are too slow and convoluted



## Helpful features

- Mechanics

  - Physical interaction during dialogue
  - Dedicated button directions for types of responses
  - Conversation recaps
  - Exposing the workings of the system for clarity (e.g. showing NPC mood)
  - Limited number of total choices an NPC can be asked
  - “News events”, their radii, spread, and evolution

- Players

  - Conversation-specific

    - Timed choices
    - Unchoices
    - Failure as success
    - Conversation interruptions (+ returns to conversation & queueing vs. interjecting)

  - Conversation-independent

    - Conversation tone choice

    - Collecting nouns and asking NPCs about said nouns
    - Stats/afflictions governing what players can say

    - Entirely different dialogue choices based on class/stats

    - Culture-influenced dialects

- NPCs

  - NPC ability to remember knowledge
  - Responsive choices to player statements
  - NPC purposes
  - Culture-influenced dialects



## Inspiration

- [BlabScript](https://www.lablablab.net/?p=701)
- [Ultima Ratio Regum](https://www.markrjohnsongames.com/games/ultima-ratio-regum)
- [Emily Short's Interactive Storytelling](https://emshort.blog/how-to-play/writing-if/my-articles/conversation)
- [A Gossip Virtual Social Network for Non Playable Characters in Role Play Games](https://ieeexplore.ieee.org/document/6680108?part=1)



## Biggest competition

- Complex
  - [Dialogue System For Unity](https://assetstore.unity.com/packages/tools/behavior-ai/dialogue-system-for-unity-11672) (biggest indie player + large number of features)
  - [articy:draft 3](https://www.articy.com/en) (AAA standard with a ton of features)
- Simplified
  - [Meet and Talk](https://assetstore.unity.com/packages/tools/visual-scripting/meet-and-talk-dialogue-system-245076) (great UI + focus on nodes)
  - [Conversa](https://assetstore.unity.com/packages/tools/visual-scripting/conversa-dialogue-system-192549) (great UI + focus on features)
  - [Node Based Dialogue System](https://assetstore.unity.com/packages/tools/game-toolkits/node-based-dialog-system-249962) (ultra-simple)



## Core idea

**Base structure:** Simple and easy-to-use dialogue system with a focus on a myriad of combinable conversation nodes

- System features
  - High quality dialogue tree UI
  - Localization
  - Toggle-able, conversation-specific progress saving
  - Toggle-able dialogue tree minimap
  - Automatic editor saving
  - Dialogue tree comments
- Base node types
  - **Choice** (player input)
  - **Response** (NPC output)
  - **Start** & **End**
- **Choice** and **Response** node modifiers/components
  - Conversation variables (from scripts)
  - Conversation variables (internal & temporary)
  - Unity Event calls
  - Voice line triggers
  - Debug log
- **Conversation variable** types
  - Player stat
  - NPC stat
  - Player & NPC relationship
  - World state



**Enhancement structure:** A set of new, advanced node modifiers/components that break up the traditional interaction styles

- **Choice** nodes
  - Timers
- **Response** nodes
  - NPC status reveal
- Both
  - Interruptions
  - Unchoice option
  - Randomness
- **Conversation variable** types
  - Player’s known “topics”
  - NPC’s maximum conversation time
  - Player & NPC cultures
  - Player & NPC conversation purposes
  - World events