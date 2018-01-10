# Tanenbaum Mac-1 Instruction Set Simulator

This project is aimed at familiarizing students with the concept of assembler style programming.


## Differences to the original

1. An additional **HALT** instruction allows the simulation to end at any point in the code
1. All values and parameters are handled as 32bit signed integers
1. The full address space consists of **65536x 32bit** signed integers, instead of the original **4096x 16** bit words
1. The program is converted into and executed as a sequence of parameterized method invocations, not actual byte code.
It can therefor neither be read nor modified by the program itself, and the entire address range may be used for data.

## Visual Editor/Simulator
The visual application can be executed using the `Tanenbaum CPU Emulator.exe` executable, or by compiling and running the `Tanenbaum CPU Emulator` project.
It allows editing syntax highlighted programs, and running them.
Actual execution is handled by a timer, and endless loops should never freeze the application, however the log may grow very large.
It may be paused at any time.

## Console Based Simulator
This console based simulator can be executed using the `trun.exe` executable, or by compiling and running the `Console Emulator` project.
It requires a path to a text file as parameter, which contains the program to execute.
If the provided program produces an endless loop, the application will endlessly flood the console with updates at maximum speed until terminated via Ctrl+C.

## Language
Commands are specified, one per line, in the form:\
`[label:] [command [parameter]] [;comment]`\
Alternatively, comments may also start with `//`.\
Commands are not case-sensitive, but the execution log will print them in upper case. They may be indented using any number of white space characters.

Jump commands (`JUMP, JNZE, JZER, JNEG, JNZE`) require the parameter to be a label, declared somewhere in the program. Labels are case-sensitive.
Labels may be put before commands (e.g. `endless-loop: JUMP endless-loop`), or into their own lines. They may contain any non-whitespace characters, including numbers.
Labels consisting only of numbers will always be interpreted as a label, however, not as an actual numeric program address.

Address commands (`ADDD, SUBD, LODD, STOD`) accept address constants as well as special addresses *a0*=0x500 through *a10*=0x50A, and *one*=0x50B.
The value at address *one* is preinitialized with the value 1, but may be changed during runtime.

## Execution
The entire address space is initialized to 0, with the exception of the special address *one* (which is set to 1).

The stack pointer is initialized to 0, such that when it is decremented, it wraps around to the last possible address (65535), and then progresses downwards.
If the stack is emptied during execution, then it will wrap around back to 0.
Unless specific stack addresses are desired, it is not necessary to explicitly initialize the stack pointer.

Execution starts with the top-most instruction specified in the program, and continues until the program either exits beyond the bottom most instruction,
or the HALT command is executed.
It may also terminate if access violations occur (trying to read/write addresses beyond 65535, etc).

During execution, each command prints the current program counter and any detected changes to the respective window or console log.

The stack pointer is logged with both its relative and absolute address.
Typically, the stack is initialized to 0 or some value via SWAP, then progresses downwards.
The relative value indicates how far it has diverged from this initial value, and is depicted as a negative number that is 0 if the stack is empty.
Practically, the stack can grow in either direction as much as it likes, so if the program does unusual things, the relative number will be off.
