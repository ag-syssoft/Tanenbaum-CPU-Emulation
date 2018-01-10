# Tanenbaum Mac-1 Instruction Set Simulator

This project is aimed at familiarizing students with the concept of processor microcode.


## Differences to the original

1. An additional **HALT** instruction allows the simulation to end at any point in the code
1. All values and parameters are handled as 32bit signed integers
1. The full address space consists of **65536x 32bit** signed integers, instead of the original **4096x 16** bit words
1. The program is converted into and executed as a sequence of actions, not actual byte code. It can therefor neither be read nor modified by the program itself.
As a result, all adresses may be used for data.

## Visual Editor/Simulator
The visual application can be executed using the `Tanenbaum CPU Emulator.exe` executable, or compiling and running the `Tanenbaum CPU Emulator` project.
It allows editing syntax highlighted programs, and running them.
Actual execution is handled by a timer, and endless loops should never freeze the application.

## Console Based Simulator
This console based simulator can be executed using the `trun.exe` executable, or compiling and running the `Console Emulator` project.
It requires a path to a text file as parameter, which contains the program to execute.
If the provided program produces an endless loop, the application will endlessly flood the console with updates at maximum speed until terminated via Ctrl+C.

## Language
Commands are specified, one per line, in the form:\
`[label:] [command [parameter]] [;comment]`\
Alternatively, comments may also start with `//`.\
Commands are not case-sensitive, but the execution log will print them in upper case. They may be indented using any number of white space characters.

Jump commands (`JUMP, JNZE, JZER, JNEG, JNZE`) require the parameter to be a label, declared somewhere in the program.
Labels may be put before commands (e.g. `endless-loop: JUMP endless-loop`), or into their own lines. They may contain any non-whitespace characters, including numbers.
Labels consisting only of numbers will always be interpreted as a label, however, not as an actual numeric program address.

Address commands (`ADDD, SUBD, LODD, STOD`) accept address constants as well as special addresses *a0*=0x500 through *a10*=0x50A, and *one*=0x50B.
The latter is preinitialized with the value 1, but may be changed during runtime.

## Execution
Parsed programs are executed by either application. Each command prints the current program counter and any detected changes to the respective window or console log.