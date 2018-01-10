# Tanenbaum Mac-1 Instruction Set Simulator

This project is aimed at familiarizing students with the concept of assembler style programming.


## Differences to the original

1. An additional **HALT** instruction allows the simulation to end at any point in the code
1. All values and parameters are handled as 32bit signed integers
1. The full address space consists of **10000x 32bit** signed integers, instead of the original **4096x 16bit** words
1. The program is converted into and executed as a sequence of referenced method invocations, not actual byte code.
The code can therefor neither be read nor modified by the program itself, and the entire address space may be used for data.

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

All commands behave as defined in *Tanenbaum, A. (1990) "Structured Computer Organisation.", Prentice Hall, 3rd edition*.
A documentation of all instructions may be found at [stvincent.edu](http://cis.stvincent.edu/carlsond/cs330/mic1/mic1doc.txt) (English), or [syssoft.blog](https://ca.syssoft.blog/wp-content/uploads/2018/01/2017W-CA06-Tanenbaum-CPU.pdf) (German).

Jump commands (`JUMP, JNZE, JZER, JNEG, JNZE`) require the parameter to be a label, declared somewhere in the program.
Labels are case-sensitive names that may contain any non-whitespace characters, including numbers.
They may be put before commands (e.g. `endless-loop: JUMP endless-loop`), or appear on their own, in which case they point to the first following command (if any).
Labels consisting only of numbers are interpreted as names, not as actual numeric program addresses.

Address commands (`ADDD, SUBD, LODD, STOD`) require either numeric parameters in the range [0,9999] or any named address (*a0*=1280 through *a10*=1290, and *one*=1291).
The value at address *one* is preinitialized with the value 1, but may be changed during runtime.
Numeric parameters must be specified as decimal numbers.

## Execution
All memory is initialized to 0, with the exception of the value at the special address *one* (which is set to 1).

The stack pointer is initialized to 0, but may be changed arbitrarily using the SWAP command.
If it reaches either extreme of the available address space, it will wrap around to the opposing extreme.
The simulation keeps track of the initial stack value (0 unless SWAP is executed), and allows to display the stack pointer relative to this initial value.
Under regular circumstances the relative stack pointer position represents the negative stack fill level.

Execution starts with the top-most instruction specified in the program, and continues until the program either exits beyond the bottom most instruction,
or executes the HALT command.
It may also terminate unintentionally if access violations occur (e.g. by trying to read/write addresses beyond 9999).

During execution, each command logs the current program counter, executed instruction text, and any detected changes.
The stack pointer, if changed, is logged with both its negative relative and absolute address (`sp := [relative]/[absolute]`, e.g. *sp := -5/9995*).
