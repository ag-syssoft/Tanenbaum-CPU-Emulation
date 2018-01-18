# Tanenbaum Mac-1 Instruction Set Simulator

This project is aimed at familiarizing students with the concept of assembler style programming.
It is written in C#.
While some parts are not as portable as they should be, the project generally runs using [Mono](http://www.mono-project.com/).


## Differences to the original

1. Some instructions were added to control the simulation
   1. **EXIT** terminates the simulation
   1. **HALT** pauses execution. Both console and visual editor halt execution, and await user input before resuming
1. All values and parameters are handled as 32bit signed integers
1. The full address space consists of **10000x 32bit** signed integers, instead of the original **4096x 16bit** words
1. The program is converted into and executed as a sequence of referenced method invocations, not actual byte code.
The code can therefor neither be read nor modified by the program itself, and the entire address space may be used for data.

## Visual Editor/Simulator
The visual application can be executed using the `Tanenbaum CPU Emulator.exe` executable, or by compiling and running the `Tanenbaum CPU Emulator` project.
It allows editing syntax highlighted programs, and running them.
Actual execution is handled by a timer, and endless loops should never freeze the application, however the log may grow very large.\
Mono can run this .exe file on non-Windows systems, but has shown visual glitches in the past.

## Console Based Simulator
This console based simulator can be executed using the `trun.exe` executable, or by compiling and running the `Console Emulator` project.
It requires a path to a text file as parameter, which contains the program to execute.
If the provided program produces an unhalted endless loop, the application will endlessly flood the console with updates until terminated via Ctrl+C.\
Mono can run this .exe file on non-Windows systems without noticeable glitches.

## Language
The language recognizes one alias declaration or program instruction per line.
The general declaration syntax is\
`[label:] [declaration] [;comment or //comment]`\
where `declaration` is an alias declaration or program instruction.
Lines may be indented using any number of non-newline whitespace characters. Likewise, any spacing between line components may be any number of non-newline whitespace characters.

### Instructions

Instructions are specified in the form:\
`instruction [parameter]`\
The character case of instructions is ignored (`SUBL` is the same as `subl`, `Subl`, or `sUbL`).
Numeric parameters must be specified as decimal numbers.
Only some instructions require parameters.


All regular instructions behave as defined in *Tanenbaum, A. (1990) "Structured Computer Organisation.", Prentice Hall, 3rd edition*.
Documentations may be found at [stvincent.edu](http://cis.stvincent.edu/carlsond/cs330/mic1/mic1doc.txt) (English), or [syssoft.blog](https://ca.syssoft.blog/wp-content/uploads/2018/01/2017W-CA06-Tanenbaum-CPU.pdf) (German).

Two custom, parameter-less instructions **HALT** (pause) and **EXIT** (end program) have been added to better control simulation.


### Labels
Labels identify the program instruction immediately following the declaration (if any).
Jump commands (`JUMP, JNZE, JZER, JNEG, JNZE`) require their parameter to be a label, declared somewhere in the program.
Jumping to a label will update the program counter to next execute the immediately following instruction (if any).
If no instructions follow, jumping to the respective label will end the program the same way **EXIT** does.

Labels are unique, case-sensitive names that may contain any non-whitespace characters.
They may be put before other declarations (e.g. `endless-loop: JUMP endless-loop`), or appear on their own, in which case they point to the first following instruction (if any).
Purely numeric labels are always interpreted as names, not as explicit program addresses.


### Aliases
Aliases represent named addresses, that reside at a fixed location, and may be initialized with a specific value at program start.\
Aliases are declared, one per line, in the form:\
`#alias name @address [=value]`\
Aliases may be declared anywhere in the code, and optionally referenced by name by any direct address instruction (`ADDD, SUBD, LODD, STOD`).
The given names are unique and case-sensitive, and may contain any non-whitespace characters. However, purely numeric names (e.g. 1234) are not allowed, since they can not be distinguished from actual addresses.
Both the address and optional initial value must be specified as decimal numbers.
The same address may be referenced by multiple aliases, but their order of value initialization is undefined if divergent.



## Execution
Before execution starts, all memory is initialized to 0, including the stack pointer, program counter, and accumulator.
If any aliases are declared with initial values, then these initializations also take place at this point.

Execution starts with the top-most instruction specified in the program, and continues until the program either exits beyond the bottom most instruction, or executes the **EXIT** instruction.
It may also terminate unintentionally if access violations occur (e.g. by trying to read/write addresses beyond 9999).
The **HALT** instruction temporarily interrupts execution and requires user input before resuming.

If the stack pointer reaches either extreme of the available address space during execution, it will wrap around to the opposite extreme.

Each instruction logs the current program counter, executed instruction text, and any detected changes.

The stack pointer, if changed, is logged with both its negative relative and absolute address (`sp := [relative]/[absolute]`, e.g. *sp := -5/9995*).
The relative address is determined from the difference between the current and initial stack pointer value, or the one last set using the **SWAP** instruction.
Under regular circumstances the relative stack pointer position represents the negative stack fill level.

