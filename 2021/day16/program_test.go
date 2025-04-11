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
	lines := [][]byte{
		[]byte("00000"),
		[]byte("00100"),
		[]byte("01110"),
		[]byte("00100"),
		[]byte("00000"),
	}

	result := Part2(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(0)
	if result != expected {
		t.Errorf("Result was incorrect, got: %v, want: %v.\n", result, expected)
	}
}
