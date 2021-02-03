local IP = "192.168.1.110"

local socket = require("socket")
local tcp = assert(socket.tcp())
local memory = require("memory.tableMemory")

local tcpError = nil
local tcpInit = false

local pageMemory = -1
local reportMem = -1
local levelMem = 0
local summonMem = -1

local pageWise = { 0x03, 0x05, 0x08, 0x0B, 0x0E, 0x11 }
local summonWise = { 0x01, 0x08, 0x10, 0x20 }

function _OnInit()
	tcpError = tcp:connect(IP, 1392)
	
	if tcpError ~= nil then
		tcp:send("[APICall::MESSAGE] - (Connected to SERVER over PORT 1392!)~")
		tcpInit = true
	end
end

function _OnFrame()
	if tcpError ~= nil and tcpInit == true then
		if ReadInt(0x01D9EAAC) ~= 0 then
			_processLevels()
			_processRegulars()
			_processForms()
			_processAbilities()
			_processSummons()
			_processReports()
			_processPages()
		end
	end
end

function _processAbilities()
	for i = 0, 4, 1
	do
		local _ability = 0x032E0FE + i * 2
		local _bitwise = memory.abilityTable[_ability]
		local _tmpMem = 0

		for z = 1, 4, 1
		do
			if ReadShort(_ability) & (_bitwise + (z - 1)) == (_bitwise + (z - 1)) then
				_tmpMem = z
			end
		end

		if _tmpMem ~= memory.abilityMem[i + 1] and _tmpMem > 0 then
			memory.abilityMem[i + 1] = _tmpMem
			tcp:send(string.format("[APICall::ABILITY] - (0x%X | 0x%X)~", 0x032E0FE + i * 2, _tmpMem))
		elseif memory.abilityMem[i + 1] ~= 0 and _tmpMem == 0 then
			memory.abilityMem[i + 1] = 0
			tcp:send(string.format("[APICall::REMOVE] - (0x%X)~", 0x032E0FE + i * 2))
		end
	end
end

function _processSummons()
	local _temp = ReadByte(0x032F1F0)

	for i = 1, 4, 1
	do
		if i > 2 then
			_temp = ReadByte(0x032F1F4)
		end

		if memory.smnTable[0x000FFFB + i] < 1 then
			if (_temp & summonWise[i]) == summonWise[i]  then
				memory.smnTable[0x000FFFB + i] = 1
				tcp:send(string.format("[APICall::ITEM] - (0x%X | 0x%X)~", 0x000FFFB + i, 1))
			elseif memory.smnTable[0x000FFFB + i] ~= 0 then
				memory.smnTable[0x000FFFB + i] = 0
				tcp:send(string.format("[APICall::REMOVE] - (0x%X)~", 0x000FFFB + i))
			end
		end
	end
end

function _processLevels()
	local _tempLvl = ReadByte(0x032E02F);
	local _tempSmn = ReadByte(0x032F056);

	local _boolSmn = false

	if _tempLvl ~= levelMem then
		levelMem = _tempLvl
		tcp:send(string.format("[APICall::LEVEL] - (0x%X | 0x%X)~", 0x032E02F, levelMem))
		
	end

	if _tempSmn ~= summonMem then
		for i = 1, 4, 1
		do
			if memory.smnTable[0x000FFFB + i] > 0 then
				_boolSmn = true
			end
		end
		
		if _boolSmn == true then
			summonMem = _tempSmn
			tcp:send(string.format("[APICall::LEVEL] - (0x%X | 0x%X)~", 0x032F056, summonMem))
			
		elseif summonMem ~= 0 then
			summonMem = 0
			tcp:send(string.format("[APICall::REMOVE] - (0x%X)~", 0x032F056))
			
		end
	end
end

function _processReports()
	local _tempMem = 0

	for i = 1, 13, 1 
	do
		if (ReadInt(0x032F1F4) & (0x20 * math.pow(2, i))) ~= 0 then
			_tempMem = _tempMem + 1
		end
	end

	if _tempMem ~= reportMemory then
		reportMemory = _tempMem
		if reportMemory == 0 then
			tcp:send(string.format("[APICall::REMOVE] - (0x%X)~", 0x032F1F4))
			
		else
			tcp:send(string.format("[APICall::ITEM] - (0x%X | 0x%X)~", 0x032F1F4, reportMemory))
			
		end
	end
end

function _processRegulars()
	for key, value in pairs(memory.regularTable) do
		if ReadByte(key) ~= value then
			memory.regularTable[key] = ReadByte(key)
			if memory.regularTable[key] == 0 then
				tcp:send(string.format("[APICall::REMOVE] - (0x%X)~", key))
				
			else
				tcp:send(string.format("[APICall::ITEM] - (0x%X | 0x%X)~", key, memory.regularTable[key] ))
				
			end
		end
	end
end

function _processPages()
	local wiseMemory = 0

	for key, value in pairs(pageWise) do
		if ReadByte(0x032C8C0) >= value then
			wiseMemory = key
		end
	end

	for key, value in pairs(pageWise) do
		if wiseMemory ~= pageMemory then
			pageMemory = wiseMemory
			if wiseMemory == 0 then
				tcp:send(string.format("[APICall::REMOVE] - (0x%X)~", 0x032C8C0))
				
			else
				tcp:send(string.format("[APICall::ITEM] - (0x%X | 0x%X)~", 0x032C8C0, pageMemory))
				
			end
		end
	end
end

function _processForms()	
	for key, value in pairs(memory.formTable) do
		if value <= 0 then
			if value == -1 then
				memory.formTable[key] = 0
				tcp:send(string.format("[APICall::REMOVE] - (0x%X)~", key))
				
			elseif memory.formWise[key] ~= 8 then
				if (ReadInt(0x032F1F0) & memory.formWise[key]) == memory.formWise[key] then
					memory.formTable[key] = 1
					tcp:send(string.format("[APICall::FORMGET] - (0x%X)~", key))
					
				elseif memory.formTable[key] ~= 0 then 
					memory.formTable[key] = 0
					tcp:send(string.format("[APICall::REMOVE] - (0x%X)~", key))
					
				end
			else
				if (ReadInt(0x032F1FA) & 8) == 8 then
					memory.formTable[key] = 1
					tcp:send(string.format("[APICall::FORMGET] - (0x%X)~", key))
					
				elseif memory.formTable[key] ~= 0 then 
					memory.formTable[key] = 0
					tcp:send(string.format("[APICall::REMOVE] - (0x%X)~", key))
					
				end
			end
		elseif ReadByte(key) ~= value then
			memory.formTable[key] = ReadByte(key)
			tcp:send(string.format("[APICall::FORMLVL] - (0x%X | 0x%X)~", key, ReadByte(key)))
		end
	end
end