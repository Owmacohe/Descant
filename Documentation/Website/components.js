let components = [
    [
        'ChangedChoice', 'None', 'Choice',
        'Changes the text of one of a ChoiceNode\'s choices if some condition is met.',
        [
            'ActorName', 'The name the actor who\'s variable is being checked',
            'ChoiceNumber', 'The index of the choice being changed (base 1)',
            'VariableType', 'The type of the variable being checked (e.g. <strong>Statistic</strong>, ' +
                            '<strong>Topic</strong>, etc.)',
            'VariableName', 'The name of the variable being checked',
            'ComparisonType', 'The type of the comparison being performed against the variable',
            'Comparison', 'The value to compare the variable against',
            'ChangeTo', 'The text to change the choice to if the comparison succeeds',
        ]
    ],
    [
        'ChangedResponse', 'None', 'Response',
        'Changes the text of a ResponseNode\'s response if some condition is met.',
        [
            'ActorName', 'The name the actor who\'s variable is being checking',
            'VariableType', 'The type of the variable being checked (e.g. <strong>Statistic</strong>, ' +
                            '<strong>Topic</strong>, etc.)',
            'VariableName', 'The name of the variable being checked',
            'ComparisonType', 'The type of the comparison being performed against the variable',
            'Comparison', 'The value to compare the variable against',
            'ChangeTo', 'The text to change the choice to if the comparison succeeds',
        ]
    ],
    [
        'Event', 'None', 'Any',
        'Calls a method from a script.',
        [
            'ObjectTag', 'The tag of the <strong>GameObject</strong> on which the script is located (optional)',
            'ScriptName', 'The name of the script on the <strong>GameObject</strong>',
            'MethodName', 'The name of the method being called',
            'Parameter', 'The parameter to pass to the method (optional)',
        ],
        'If <strong>ObjectTag</strong> is empty, the first found script in the scene ' +
        'with the given name is called instead.<br><br>If the <strong>ScriptName</strong> or <strong>MethodName</strong> ' +
        'are empty, no method is called.<br><br>While <strong>Parameter</strong> is technically of type ' +
        '<strong>string</strong>, <strong>Integers</strong> and <strong>Floats</strong> can also be written, and ' +
        'will be parsed accordingly.<br><br>Currently, methods with more than one parameter cannot be called.'
    ],
    [
        'Interruptable', 'None', 'Response',
        'Allows for the dialogue to be abruptly stopped with a key or button press. Once this happens, a method ' +
        'from a script can be called.',
        [
            'KeyCode', 'The lowercase name of the key press to check for (e.g. "e", "space", etc.) (optional)',
            'ButtonName', 'The name of the button press to check for (e.g. "Fire1", etc.) (optional)',
            'ObjectTag', 'The tag of the <strong>GameObject</strong> on which the script is located (optional)',
            'ScriptName', 'The name of the script on the <strong>GameObject</strong> (optional)',
            'MethodName', 'The name of the method being called (optional)',
            'Parameter', 'The parameter to pass to the method (optional)',
        ],
        'If <strong>ObjectTag</strong> is empty, the first found script in the scene ' +
        'with the given name is called instead.<br><br>If the <strong>ScriptName</strong> or <strong>MethodName</strong> ' +
        'are empty, no method is called.<br><br>While <strong>Parameter</strong> is technically of type ' +
        '<strong>string</strong>, <strong>Integers</strong> and <strong>Floats</strong> can also be written, and ' +
        'will be parsed accordingly.<br><br>Currently, methods with more than one parameter cannot be called.'
    ],
    [
        'LockedChoice', 'None', 'Choice',
        'Removes one of a ChoiceNode\'s choices if some condition is met.',
        [
            'ActorName', 'The name the actor who\'s variable is being checked',
            'ChoiceNumber', 'The index of the choice being removed (base 1)',
            'VariableType', 'The type of the variable being checked (e.g. <strong>Statistic</strong>, ' +
                            '<strong>Topic</strong>, etc.)',
            'VariableName', 'The name of the variable being checked',
            'ComparisonType', 'The type of the comparison being performed against the variable',
            'Comparison', 'The value to compare the variable against',
        ]
    ],
    [
        'Log', 'None', 'Any',
        'Prints a <strong>Debug.Log</strong> to the console.',
        [
            'Message', 'The message to log',
        ]
    ],
    [
        'PortraitChange', 'None', 'Any',
        'Changes and actor\'s portrait.',
        [
            'PlayerPortrait', 'Whether the portrait being changed is the player\'s or the NPC\'s.',
            'ChangeType', 'The change being made to the portrait (set, enabled, or disabled)',
            'PortraitName', 'The name of the new portrait to be set (optional)',
        ],
        'For a portrait to be set, a <strong>Sprite</strong> with the corresponding name must be passed into the ' +
        '<strong>DescantDialogueTrigger</strong>\'s <strong>portrait</strong> property.'
    ],
    [
        'RandomizedChoice', '1', 'Choice',
        'Randomizes/shuffles a ChoiceNode\'s choices.',
        [],
        'As a best practice, always make sure you only add this Component to a node <em>after</em> all other ' +
        'Components have been added to that node (especially after ones that affect a specific choice).'
    ],
    [
        'RelationshipChange', 'None', 'Any',
        'Changes one of an actor\'s relationships.',
        [
            'FirstActorName', 'The name of the actor who\'s relationship is being changed',
            'SecondActorName', 'The name of the actor that the relationship corresponds to',
            'OperationType', 'The change being made to the relationship (increase, decrease, or set)',
            'OperationValue', 'The value to change the relationship by',
        ],
        'Changing the first actor\'s relationship to the second will <em>not</em> change the second\'s relationship ' +
        'to the first.'
    ],
    [
        'StatisticChange', 'None', 'Any',
        'Changes an one of actor\'s statistics.',
        [
            'ActorName', 'The name the actor who\'s statistic is being changed',
            'StatisticName', 'The name of the statistic being changed',
            'OperationType', 'The change being made to the statistic (increase, decrease, or set)',
            'OperationValue', 'The value to change the statistic by',
        ]
    ],
    [
        'StatisticReveal', 'None', 'Any',
        'Calls a method from a script, passing it an actor\'s statistic (e.g. to display how an NPC\'s mood might ' +
        'increase/decrease during a dialogue).',
        [
            'ActorName', 'The name the actor who\'s variable is being revealed',
            'StatisticName', 'The name of the statistic being revealed',
            'ObjectTag', 'The tag of the <strong>GameObject</strong> on which the script is located (optional)',
            'ScriptName', 'The name of the script on the <strong>GameObject</strong>',
            'MethodName', 'The name of the method being called',
        ],
        'If <strong>ObjectTag</strong> is empty, the first found script in the scene ' +
        'with the given name is called instead.<br><br>If the <strong>ScriptName</strong> or <strong>MethodName</strong> ' +
        'are empty, no method is called.<br><br>Currently, methods with more than one parameter cannot be called.'
    ],
    [
        'TimedChoice', '1', 'Choice',
        'Imposes a time restriction for choosing a TimedChoice\'s choice. Each FixedUpdate frame that the time is ' +
        'counting down, a method from a script can be called, passing the timer completion percentage (e.g. to ' +
        'update a timer UI). When the time reaches 0, a choice is automatically made, and another method can be ' +
        'called (e.g. to initiate a point penalty).',
        [
            'Time', 'The amount of time the player has to choose (in seconds)',
            'ChoiceToPick', 'The index of the choice to pick if the timer runs out (base 1)',
            'ObjectTag', 'The tag of the <strong>GameObject</strong> on which the script is located (optional)',
            'ScriptName', 'The name of the script on the <strong>GameObject</strong>',
            'TimerMethodName', 'The name of the method called while the timer is counting down (optional)',
            'FinishedMethodName', 'The name of the method called when the timer reaches 0 (optional)',
        ],
        'If <strong>ObjectTag</strong> is empty, the first found script in the scene ' +
        'with the given name is called instead.<br><br>If the <strong>ScriptName</strong> is empty, no method is ' +
        'called. If <strong>TimerMethodName</strong> is empty, it isn\'t called. If <strong>FinishedMethodName</strong> ' +
        'is empty, it isn\'t called.<br><br>Currently, methods with more than one parameter cannot be called.'
    ],
    [
        'TopicChange', 'None', 'Response',
        'Changes one of an actor\'s topics.',
        [
            'ActorName', 'The name the actor who\'s topic is being changed',
            'TopicName', 'The name of the topic being changed',
            'ChangeType', 'The change being made to the topic (add or remove)',
        ]
    ],
];

window.onload = function () {
    let components_parent = document.getElementById('components');

    for (let i in components) {
        let component = document.createElement('div');
        component.setAttribute('class', 'flex column start component');
        component.id = components[i][0].toLowerCase();
        components_parent.appendChild(component);

        let variables = '';

        for (let j = 0; j < components[i][4].length; j++)
            if (j % 2 === 0) variables += '<p><strong>' + components[i][4][j] + '</strong>: ' + components[i][4][j + 1] + '</p>';

        component.innerHTML =
            '<h3 style="text-decoration: underline;"><strong>' + components[i][0] + '</strong></h3>' +
            '<p style="margin-bottom: 30px">' + components[i][3] + '</p>' +
            '<p><strong>Max Quantity:</strong> ' + components[i][1] + '</p>' +
            '<p><strong>Node Type(s):</strong> ' + components[i][2] + '</p>' +
            (components[i][4].length === 0 ? '' : ('<p style="margin-top: 30px">Variables:</p>' + variables)) +
            (components[i].length < 6 ? ''
                : '<p style="margin-top: 30px">Disclaimers:</p><p class="important">' + components[i][5] + '</p>');
    }

    let anchors = document.querySelectorAll('a');

    for (const j in anchors) {
        try {
            anchors[j].setAttribute('rel', 'noreferrer noopener');
            anchors[j].setAttribute('target', '_blank');
        }
        catch { }
    }
};