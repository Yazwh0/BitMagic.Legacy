# BitMagic the Compiler

**Note: Not all opcodes are supported. Please see the section at the bottom.**

The compiler takes the output of the Template Engine, or hand written `.bmasm` files and compiles it into machine code that can be run on the end machine or emulator.

The compiler is simpler than most compilers as it lacks features like 'Macros' or 'libraries'.

## The Basics

A line in the `bmasm` file must be one of the following to be considered valid:

- Start with a valid opcode.
- Start with a '`.`' to indicate a verb to the compiler.
- Start with a '`;`' for a comment so the line is ignored.
- Be blank.

## Compiler Verbs

Lines that start with a '.' indicate a verb to the compiler. Some have parameters that can either be comma separated, specified with `name=value` pairs, or a mix of the two. Eg `.segment rom, address = 0xA000`.

| Verb | Parameters |Description |
| --- | --- | --- |
| .*label*: | | Define a label that can be used in code. Label name cannot be empty. |
| .segment | name, address, filename, maxsize | Switches and defines a segment. |
| .endsegment | | Switches to the default segment. |
| .scope | name | Switches to a named scope within the current segment. |
| .endscope | | Switches to a new default scope. |
| .proc | name | Switches to the named procedure. The procedure's name and address is stored as a constant. |
| .endproc | | Switches to a new default procedure. |
| .const | name, value | Sets a constant value. |
| .importfile | filename | Imports a file as if it were included in the original document. A file will only be included once. |
| .org | address | Sets the current state to the target address. |
| .pad | size | Increases the address by size bytes. |
| .align | boundary | Pads the current state to the next nearest boundary. (Where '`address % boundary`' is zero.) |
| .byte | *values* | Adds the bytes to the application. |
| .word | *values* | Adds the words (little endian) to the application. |

## Import

A project can be broken into different files to ease development. To import a file you can use the `.importfile` verb.

This will include the whole of the files contents at the point of the import verb. There is no segregation performed by the compiler, so take care and use `.scope` and importantly `.endscope` to ensure there are no problems with overwriting definitions.

The aim is to keep this functionality simple for things like organising a project, while libraries and code reuse are achieved by the template engine.

## Scopes

Constant values, including the addresses for procedures are scoped by Globals -> Scope -> Procedure.

When the compiler is looking for a value, it will consider the current procedure first, before going up the tree to the scope, then globals.

You can break out of that by using colons to indicate the scope of what you're trying to find. Each layer is separated by a colon, so for fully qualified value it would be `App:ScopeName:ProcName:ValueName`. If the procedure isn't named then its name will not be considered, making the lookup `App:ScopeName:ValueName`.

The compiler can output the list of variables to make it easier to understand.

The code below shows how to get the values that are out of scope.

```asm
.scope 
    .proc A
        .const xyz = $ff
    ;.endproc no .endproc necessary, but its advised!
.endscope

.scope
    .proc B
        lda App::A:xyz ; all these are $ff
        lda App:::xyz
        lda :::xyz
        lda ::A:xyz
        lda :A:xyz
        lda :xyz
    .endproc

; no .endscope necessary, its assumed with a .scope

.scope example_c
    .const xyz = $f0

    .proc C
        lda App::A:xyz ; all these are $ff
        lda App:::xyz
        lda :::xyz
        lda ::A:xyz
        lda :A:xyz
        ;lda :xyz breaks, not unique
        lda xyz ; will be $f0 from this scope
    .endproc

    lda App::A:xyz ; all these are $ff
    lda App:::xyz
    lda :::xyz
    lda ::A:xyz
    lda :A:xyz
    ;lda :xyz breaks, not unique
    lda example_c:xyz ; will be $f0 from this scope, note no procedure name
    lda xyz ; will be $f0 from this scope
.endscope
```

### Expressions

For values within the assembler you can use expressions as you'd expect. `^`, `<` and `>` will work.

The Expression code does need to be reworked as there are some issues with it. If you just use simple maths it should be fine.

Currently expressions can only be in the asm code, not when defining a constant. This is on the todo list.

### Unsupported 65c02 OpCodes

| Byte | OpCode | Mode |
| --- | --- | --- |
| $0F | BBR0 | |
| $1F | BBR1 | |
| $2F | BBR2 | |
| $3F | BBR3 | |
| $4F | BBR4 | |
| $5F | BBR5 | |
| $6F | BBR6 | |
| $7F | BBR7 | |
| $8F | BBS0 | |
| $9F | BBS1 | |
| $AF | BBS2 | |
| $BF | BBS3 | |
| $CF | BBS4 | |
| $DF | BBS5 | |
| $EF | BBS6 | |
| $FF | BBS7 | |

### Todo

- Implement all OpCodes.
- Create object files to aid basic debugging.
- Rework Expression code.
- Allow Expressions in constant creation.
- Add machine \ CPU switches to make compiler useful for different machines.
