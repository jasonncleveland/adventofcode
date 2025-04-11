package day16

import (
	"testing"
)

func TestPart1(t *testing.T) {
	tests := []struct {
		input    [][]byte
		expected int64
	}{
		{
			[][]byte{
				[]byte("8A004A801A8002F478"),
			},
			16,
		},
		{
			[][]byte{
				[]byte("620080001611562C8802118E34"),
			},
			12,
		},
		{
			[][]byte{
				[]byte("C0015000016115A2E0802F182340"),
			},
			23,
		},
		{
			[][]byte{
				[]byte("A0016C880162017C3686B18A3D4780"),
			},
			31,
		},
	}

	for _, test := range tests {
		result := Part1(test.input)
		if result == -1 {
			t.Fatal("Function or tests for part 1 not implemented")
		}

		if result != test.expected {
			t.Errorf("Result was incorrect, got: %d, want: %d.\n", result, test.expected)
		}
	}
}

func TestPart2(t *testing.T) {
	tests := []struct {
		input    [][]byte
		expected int64
	}{
		{
			[][]byte{
				[]byte("C200B40A82"),
			},
			3,
		},
		{
			[][]byte{
				[]byte("04005AC33890"),
			},
			54,
		},
		{
			[][]byte{
				[]byte("880086C3E88112"),
			},
			7,
		},
		{
			[][]byte{
				[]byte("CE00C43D881120"),
			},
			9,
		},
		{
			[][]byte{
				[]byte("D8005AC2A8F0"),
			},
			1,
		},
		{
			[][]byte{
				[]byte("F600BC2D8F"),
			},
			0,
		},
		{
			[][]byte{
				[]byte("9C005AC2F8F0"),
			},
			0,
		},
		{
			[][]byte{
				[]byte("9C0141080250320F1802104A08"),
			},
			1,
		},
	}

	for _, test := range tests {
		result := Part2(test.input)
		if result == -1 {
			t.Fatal("Function or tests for part 2 not implemented")
		}

		if result != test.expected {
			t.Errorf("Result was incorrect, got: %d, want: %d.\n", result, test.expected)
		}
	}
}
