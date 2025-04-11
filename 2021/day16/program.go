package day16

import (
	"fmt"
	"strconv"
	"strings"
	"time"

	"github.com/jasonncleveland/adventofcode/2021/utils"
)

func Run(fileName string) {
	var start time.Time = time.Now()
	lines := utils.ReadFileLines(fileName)
	fmt.Printf("File read: %s\n", time.Since(start))

	start = time.Now()
	part1 := Part1(lines)
	fmt.Printf("Part 1: %d (%s)\n", part1, time.Since(start))

	start = time.Now()
	part2 := Part2(lines)
	fmt.Printf("Part 2: %d (%s)\n", part2, time.Since(start))
}

func Part1(lines [][]byte) int64 {
	binaryString := ConvertHexToBinary(lines[0])

	var index int = 0
	return ParsePacket(binaryString, &index)
}

func Part2(lines [][]byte) int64 {
	return -1
}

func ConvertHexToBinary(hexString []byte) string {
	lookupTable := map[byte]string{
		'0': "0000",
		'1': "0001",
		'2': "0010",
		'3': "0011",
		'4': "0100",
		'5': "0101",
		'6': "0110",
		'7': "0111",
		'8': "1000",
		'9': "1001",
		'A': "1010",
		'B': "1011",
		'C': "1100",
		'D': "1101",
		'E': "1110",
		'F': "1111",
	}

	var builder strings.Builder
	for _, hex := range hexString {
		builder.WriteString(lookupTable[hex])
	}
	return builder.String()
}

func ParsePacket(packet string, index *int) int64 {
	version, _ := strconv.ParseInt(packet[*index:*index+3], 2, 8)
	*index += 3
	typeId, _ := strconv.ParseInt(packet[*index:*index+3], 2, 8)
	*index += 3

	total := version
	switch typeId {
	case 4:
		// Literal value
		var result strings.Builder
		for {
			group := packet[*index : *index+5]
			*index += 5
			result.WriteString(group[1:])
			if group[0] == '0' {
				break
			}
		}
		strconv.ParseInt(result.String(), 2, 64)
	default:
		// Sub-packets
		lengthTypeId := packet[*index] - '0'
		*index++
		if lengthTypeId == 0 {
			bitLength, _ := strconv.ParseInt(packet[*index:*index+15], 2, 64)
			*index += 15
			finalIndex := *index + int(bitLength)
			for *index < finalIndex {
				total += ParsePacket(packet, index)
			}
		} else {
			subPacketCount, _ := strconv.ParseInt(packet[*index:*index+11], 2, 64)
			*index += 11
			for range subPacketCount {
				total += ParsePacket(packet, index)
			}
		}
	}
	return total
}
