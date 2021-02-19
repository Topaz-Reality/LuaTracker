local tableMemory =  {}

tableMemory.regularTable = 
{
	[0x032F0C4] = -1,
	[0x032F0C5] = -1,
	[0x032F0C6] = -1,
	[0x032F0C7] = -1,
	[0x032F0FF] = -1,
	[0x032F100] = -1,

	[0x032F1E2] = -1,
	[0x032F1E3] = -1,
	[0x032F1E4] = -1,

	[0x032F1C4] = -1
}

tableMemory.formTable = 
{
	[0x032EE26] = -1,
	[0x032EE5E] = -1,
	[0x032EE96] = -1,
	[0x032EECE] = -1,
	[0x032EF06] = -1
}

tableMemory.smnTable = 
{
	[0x000FFFC] = -1,
	[0x000FFFD] = -1,
	[0x000FFFE] = -1,
	[0x000FFFF] = -1
}

tableMemory.abilityTable = 
{
	[0x032E0FE] = 0x005E,
	[0x032E100] = 0x0062,
	[0x032E102] = 0x0234,
	[0x032E104] = 0x0066,
	[0x032E106] = 0x006A
}

tableMemory.abilityMem = 
{
	[0x032E0FE] = -1,
	[0x032E100] = -1,
	[0x032E102] = -1,
	[0x032E104] = -1,
	[0x032E106] = -1
}

tableMemory.formWise = 
{
	[0x032EE26] = 2,
	[0x032EE5E] = 4,
	[0x032EE96] = 8,
	[0x032EECE] = 64,
	[0x032EF06] = 16
}

return tableMemory