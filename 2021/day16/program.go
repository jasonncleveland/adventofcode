package day16

import (
	"fmt"
	"math"
	"strconv"
	"strings"
	"time"

	"advent-of-code-2021/utils"
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

	var total int64 = 0
	var index int = 0
	ParsePacket(binaryString, &index, &total)

	return total
}

func Part2(lines [][]byte) int64 {
	binaryString := ConvertHexToBinary(lines[0])

	var total int64 = 0
	var index int = 0
	return ParsePacket(binaryString, &index, &total)
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

func ParsePacket(packet string, index *int, total *int64) int64 {
	version, _ := strconv.ParseInt(packet[*index:*index+3], 2, 8)
	*index += 3
	typeId, _ := strconv.ParseInt(packet[*index:*index+3], 2, 8)
	*index += 3

	*total += version

	switch typeId {
	case 0:
		// Sum of all sub-packets
		subPackets := GetSubPackets(packet, index, total)
		var sum int64 = 0
		for _, subPacket := range subPackets {
			sum += subPacket
		}
		return sum
	case 1:
		// Product of all sub-packets
		subPackets := GetSubPackets(packet, index, total)
		var product int64 = 1
		for _, subPacket := range subPackets {
			product *= subPacket
		}
		return product
	case 2:
		// Minimum of all sub-packets
		subPackets := GetSubPackets(packet, index, total)
		var min int64 = math.MaxInt64
		for _, subPacket := range subPackets {
			if subPacket < min {
				min = subPacket
			}
		}
		return min
	case 3:
		// Maximum of all sub packets
		subPackets := GetSubPackets(packet, index, total)
		var max int64 = math.MinInt64
		for _, subPacket := range subPackets {
			if subPacket > max {
				max = subPacket
			}
		}
		return max
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
		value, _ := strconv.ParseInt(result.String(), 2, 64)
		return value
	case 5:
		// Greater than (1 if first sub-packet is greater than second sub-packet else 0)
		subPackets := GetSubPackets(packet, index, total)
		if subPackets[0] > subPackets[1] {
			return 1
		} else {
			return 0
		}
	case 6:
		// Less than (1 if first sub-packet is less than second sub-packet else 0)
		subPackets := GetSubPackets(packet, index, total)
		if subPackets[0] < subPackets[1] {
			return 1
		} else {
			return 0
		}
	case 7:
		// Equal to (1 if first sub-packet is equal to second sub-packet else 0)
		subPackets := GetSubPackets(packet, index, total)
		if subPackets[0] == subPackets[1] {
			return 1
		} else {
			return 0
		}
	default:
		panic(fmt.Errorf("invalid packet type found [%d]. must be between 0-7", typeId))
	}
}

func GetSubPackets(packet string, index *int, total *int64) []int64 {
	var subPackets []int64

	lengthTypeId := packet[*index] - '0'
	*index++

	if lengthTypeId == 0 {
		bitLength, _ := strconv.ParseInt(packet[*index:*index+15], 2, 64)
		*index += 15
		finalIndex := *index + int(bitLength)
		for *index < finalIndex {
			subPackets = append(subPackets, ParsePacket(packet, index, total))
		}
	} else {
		subPacketCount, _ := strconv.ParseInt(packet[*index:*index+11], 2, 64)
		*index += 11
		for range subPacketCount {
			subPackets = append(subPackets, ParsePacket(packet, index, total))
		}
	}

	return subPackets
}
