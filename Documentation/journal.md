## Tuesday, November 7^th^

It’s been a little while since I last check in, and a lot of organizational changes, QOL tweaks, and bug fixes have come along. Most importantly, I finally settled on an appropriate package folder structure. I’d been messing around with a 2-folder and 2-folder systems with just as many Assembly Definitions, but I’ve settled now on what I like the best. It allows for `Runtime` scripts to reference `Editor` scripts, and allows for them to both reference `Components` scripts. When the `Components` will have to start referencing `Runtime` though, that’ll be a problem. We’ll figure it out as it comes. I also added a bunch of comments to new scripts, converted my Descant Graph files from `.desc` to `.desc.json`, and hid the `Components` scripts for the mean time, as I prepare to push a `v0.1.1` build up to GitHub.

The reason for all this organization is the fact that I’ll be demoing Descant in its current state and talking about the future design goals with this semester’s CART 415 class. I had talked with both Jonathan and Jongwoo Kim regarding possibly doing a Descant workshop in their more practice-based classes. The students would benefit from using my system for their game jams (albeit in its simplified state), and I would get feedback regarding usability and my future goals. I was worried about getting it all working and buildable by tomorrow, when I’ll be doing the workshop, but everything is working perfectly now, and I couldn’t be happier!

The next goals after the workshop are to continue developing the Component system by allowing the `Components` to be saved to the `DescantGraphData` file. After that, it’s onto run-time implementation, which will be no small feat.



## Wednesday, October 25^th^

I’ve decided to focus on the node components for the moment. Once I get them down and working, then I can add the Actor and Conversation components if I want to / have time to. I’ve been able to get most of them half-written, I started work on saving them, and I’ve added probably 90% of their UI implementation in the Editor. Now I just need to read the data when saving, load it properly, and get it working in runtime. I’m quite pleased with my progress, though I know that I need to start interviewing / demoing the software soon, to get an idea regarding the UX and feasibility. I’ll want to meet with Jonathan soon too to discuss my progress.

As a sidenote, I also reorganized the project so that it only uses one Assembly Definition. This way I can avoid circular dependancy. This wasn’t an issue before, but now that I have the Components going, it’s come up. The Components use both the Editor and the Runtime, and those both use the Components. So I had to put them all in the same Assembly Definition.



## Monday, October 16^th^

I’ve added a new file to the documentation, detailing all of the ‘components’ that I will be writing for the system. I have node, actor, and full conversation components. Theoretically, these will be able to be added to their respective types in a totally free, totally modular fashion. Presently, I’m just getting a hold of how I’m going to program it. I’ve added a bunch of TODOs, and have started on about half of the node components. Some are invokable, meaning that they trigger some effect when the node they’re attached to is reached, but some are just for data storage, to be accessed by other scripts which might want to make use of their properties (e.g. a `TimedChoice` component won’t ever trigger anything itself, but its data members will be necessary for the `DescantConversationController` scripts to reference when updating countdowns and moving to the next node).

I’m feeling really good about what I’ve got done so far, but it’s definitely going to be a lot of work. Maybe more than I thought. I think it might be best to push my prototype date a week or so into the future, giving me time to get these all working. Instead of doing user testing right away, I’ll run a seminar where folks can comment openly, and I can discuss it in a casual manner. Until then, I’ll keep coding away at these components.



## Monday, October 9^th^

The base system has finally been settled! I have the Editor script all done (save for a few QOL or functionality tweaks), the saving works pretty well, and I’m now getting started on the Runtime scripts. While I do that, I’ll be working on the unique aspects of Descant. That’s where the real creativity will start to shine through, and I’ll be journalling a lot more (I hope). Presently I’m going to try writing one of these journals every 2-3 commits. To coincide with my general pattern of milestones. I’ll also be ramping up my explanations in commit messages, going beyond the standard dry descriptions of changes.

I’m nearing the proof-of-concept milestone that I played out for myself originally. I’m feeling good about my progress, though I know that the ‘components’ system that I had dreamed up will take some finagling to get working. I think that my next steps for the project are to make a ‘build’ of the current package, build a super simple runtime testing interface, then get going on the ‘components’ system. On Jonathan’s recommendation, I’ll also try to classify and narrow my sights down to just 2-3 for each of the node, graph, and actor scopes. I’ll be doing that soon too I hope.