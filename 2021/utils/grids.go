package utils

func ParseIntGrid(lines [][]byte) [][]int64 {
	var data [][]int64

	for _, line := range lines {
		var bytes []int64
		for _, bit := range line {
			bytes = append(bytes, int64(bit-byte('0')))
		}
		data = append(data, bytes)
	}

	return data
}
