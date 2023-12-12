## Base system design



### System features

- High quality dialogue tree UI
- Localization
- Toggle-able, conversation-specific progress saving
- Automatic editor saving



### Node types

- **Choice** (player input)
- **Response** (NPC output)
- **Start** & **End**



### Base node components

- Conversation variables (from scripts)
- Conversation variables (internal & temporary)
- Unity Event calls
- Voice line triggers
- Debug log



### Conversation variables

- Player stat
- NPC stat
- Player & NPC relationship
- World state



### Optional features

- Toggle-able dialogue tree minimap
- Dialogue tree comments



## Enhancement design



### Enhanced node components

- Timers (**Choice** nodes)
- NPC status reveal (**Response** nodes)
- Interruptions
- Unchoice option
- Randomness



### Conversation components

- Player’s known “topics”
- NPC’s maximum conversation time
- Player & NPC cultures
- Player & NPC conversation purposes
- World events