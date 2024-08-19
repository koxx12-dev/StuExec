/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
define(["require", "exports"], function (require, exports) {
    'use strict';
    Object.defineProperty(exports, "__esModule", {value: true});
    exports.conf = {
        comments: {
            lineComment: '--',
            blockComment: ['--[[', ']]'],
        },
        brackets: [
            ['{', '}'],
            ['[', ']'],
            ['(', ')'],
        ],
        autoClosingPairs: [
            {open: '{', close: '}'},
            {open: '[', close: ']'},
            {open: '(', close: ')'},
            {open: '"', close: '"'},
            {open: "'", close: "'"},
        ],
        surroundingPairs: [
            {open: '{', close: '}'},
            {open: '[', close: ']'},
            {open: '(', close: ')'},
            {open: '"', close: '"'},
            {open: "'", close: "'"},
        ],
        folding: {
            markers: {
                start: new RegExp("^\\s*//\\s*(?:(?:#?region\\b)|(?:<editor-fold\\b))"),
                end: new RegExp("^\\s*//\\s*(?:(?:#?endregion\\b)|(?:</editor-fold>))")
            }
        },
    };
    exports.language = {
        defaultToken: '',
        tokenPostfix: '.lua',
        keywords: [
            // https://developer.roblox.com/en-us/articles/Variables
            'and', 'break', 'continue', 'do', 'else', 'elseif',
            'end', 'false', 'for', 'function', 'if',
            'in', 'local', 'nil', 'not', 'or',
            'repeat', 'return', 'then', 'true', 'until',
            'while',
        ],
        brackets: [
            {token: 'delimiter.bracket', open: '{', close: '}'},
            {token: 'delimiter.array', open: '[', close: ']'},
            {token: 'delimiter.parenthesis', open: '(', close: ')'}
        ],
        globals: [
            'print', 'error', 'warn', 'require', 'game', 'assert',
            'rawset', 'rawget', 'rawequal',
            'coroutine', 'debug', 'table', 'pairs', 'ipairs', 'bit', 'bit32', 'math', 'utf8', 'os',
            'collectgarbage', 'getfenv', 'getmetatable',
            'loadstring', 'newproxy', 'next',
            'pcall', 'select', 'setfenv',
            'setmetatable', 'tonumber', 'tostring', 'type', 'xpcall', '_G',
            'shared', 'delay', 'spawn', 'tick', 'typeof', 'wait',
            'Enum', 'script', 'workspace', 'printidentity',
            
            "Websocket", // (UNC) Websocket
            "Drawing", "cleardrawcache", "getrenderproperty", "isrenderobj", "setrenderproperty", // (UNC) Drawing
            "cloneref", "compareinstances", // (UNC) Cache
            "checkcaller", "clonefunction", "getcallingscript", "hookfunction", "iscclosure", "islclosure", "isexecutorclosure", "loadstring", "newcclosure", // (UNC) Closures
            "consoleclear", "rconsoleclear", "consolecreate", "rconsolecreate", "consoledestroy", "rconsoledestroy", "consoleinput", "rconsoleinput", "consoleprint", "rconsoleprint", "consolesettitle", "rconsolesettitle", // (UNC) Console
            "crypt", "base64_encode", "base64", "base64_decode", // (UNC) Crypto
            "debug", // (UNC) Debug
            "readfile", "listfiles", "writefile", "makefolder", "appendfile", "isfile", "isfolder", "delfile", "delfolder", "loadfile", "dofile", // (UNC) Filesystem
            "isrbxactive", "mouse1click", "mouse1press", "mouse1release", "mouse2click", "mouse2press", "mouse2release", "mousemoveabs", "mousemoverel", "mousescroll", // (UNC) Input
            "fireclickdetector", "getcallbackvalue", "getconnections", "getcustomasset", "gethiddenproperty", "gethui", "getinstances", "getnilinstances", "isscriptable", "sethiddenproperty", "setrbxclipboard", "setscriptable", // (UNC) Instance
            "getrawmetatable", "hookmetamethod", "getnamecallmethod", "isreadonly", "setrawmetatable", "setreadonly", // (UNC) Metatable
            "identifyexecutor", "lz4compress", "lz4decompress", "messagebox", "queue_on_teleport", "request", "setclipboard", "setfpscap", // (UNC) Misc
            "getgc", "getgenv", "getloadedmodules", "getrenv", "getrunningscripts", "getscriptbytecode", "getscriptclosure", "getscripthash", "getscripts", "getsenv", "getthreadidentity", "setthreadidentity", // (UNC) Script
        ],
        types: [
            'Axes', 'BrickColor', 'CatalogSearchParams', 'CFrame', 'Color3', 'ColorSequence',
            'ColorSequenceKeypoint', 'DateTime', 'Drawing', 'DockWidgetPluginGuiInfo', 'Faces',
            'Instance', 'NumberRange', 'NumberSequence', 'NumberSequenceKeypoint', 'PathWaypoint',
            'PhysicalProperties', 'Random', 'Ray', 'RaycastParams', 'RaycastResult', 'Rect',
            'Region3', 'Region3int16', 'TweenInfo', 'UDim', 'UDim2', 'Vector2', 'Vector2int16',
            'Vector3', 'Vector3int16',
        ],
        operators: [
            '+', '-', '*', '/', '%', '^', '#', '..',
            '==', '~=', '<=', '>=', '<', '>', 'or', 'and', 'not',
            '=', '+=', '-=', '*=', '/=', '%=', '^=', "..=",
        ],

        symbols: /[=><!~?:&|+\-*\/\^%]+/,
        escapes: /\\(?:[abfnrtv\\"']|x[0-9A-Fa-f]{1,4}|u[0-9A-Fa-f]{4}|U[0-9A-Fa-f]{8})/,

        tokenizer: {
            root: [
                [/[a-zA-Z_]\w*/, {
                    cases: {
                        '@keywords': {token: 'keyword.$0'},
                        '@globals': {token: 'global'},
                        '@types': {token: 'type'},
                        '@default': 'identifier'
                    }
                }],
                {include: '@whitespace'},
                [/(,)(\s*)([a-zA-Z_]\w*)(\s*)(:)(?!:)/, ['delimiter', '', 'key', '', 'delimiter']],
                [/({)(\s*)([a-zA-Z_]\w*)(\s*)(:)(?!:)/, ['@brackets', '', 'key', '', 'delimiter']],
                [/[{}()\[\]]/, '@brackets'],
                [/@symbols/, {
                    cases: {
                        '@operators': 'delimiter',
                        '@default': ''
                    }
                }],
                [/\d*\.\d+([eE][\-+]?\d+)?/, 'number.float'],
                [/0[xX][0-9a-fA-F_]*[0-9a-fA-F]/, 'number.hex'],
                [/\d+?/, 'number'],
                [/[;,.]/, 'delimiter'],
                [/"([^"\\]|\\.)*$/, 'string.invalid'],
                [/'([^'\\]|\\.)*$/, 'string.invalid'],
                [/"/, 'string', '@string."'],
                [/'/, 'string', '@string.\''],
            ],
            whitespace: [
                [/[ \t\r\n]+/, ''],
                [/--\[([=]*)\[/, 'comment', '@comment.$1'],
                [/--[tT][oO][-]?[dD][oO].*$/, 'comment.todo'],
                [/--.*$/, 'comment'],
            ],
            comment: [
                [/[^\]]+/, 'comment'],
                [/\]([=]*)\]/, {
                    cases: {
                        '$1==$S2': {token: 'comment', next: '@pop'},
                        '@default': 'comment'
                    }
                }],
                [/./, 'comment']
            ],
            string: [
                [/[^\\"']+/, 'string'],
                [/@escapes/, 'string.escape'],
                [/\\./, 'string.escape.invalid'],
                [/["']/, {
                    cases: {
                        '$#==$S2': {token: 'string', next: '@pop'},
                        '@default': 'string'
                    }
                }
                ]
            ],
        },
    };
});
