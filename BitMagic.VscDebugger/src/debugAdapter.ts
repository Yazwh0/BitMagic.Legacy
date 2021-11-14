console.log("debug adapter starting");
console.error("boo")

var exec = require('child_process').exec;

exec(`D:/Documents/Source/BitMagic/BitMagic/bin/Debug/net5.0/BitMagic.exe --razor-file="${process.argv[0]}" razor compile emulate > c:/temp/foo.txt`);
