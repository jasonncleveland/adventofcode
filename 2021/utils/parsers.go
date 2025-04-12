package utils

func ParseNumber(bytes []byte) int64 {
	var magnitude int64 = 1
	var sign int64 = 1
	var number int64 = 0

	if bytes[0] == '-' {
		sign = -1
		bytes = bytes[1:]
	}

	for index := range bytes {
		number += int64(bytes[len(bytes)-1-index]-byte('0')) * magnitude
		magnitude *= 10
	}
	return number * sign
}
