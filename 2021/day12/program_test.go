package day12

import (
	"reflect"
	"testing"
)

func TestPart1(t *testing.T) {
	tests := []struct {
		input    [][]byte
		expected int64
	}{
		{
			[][]byte{
				[]byte("start-A"),
				[]byte("start-b"),
				[]byte("A-c"),
				[]byte("A-b"),
				[]byte("b-d"),
				[]byte("A-end"),
				[]byte("b-end"),
			},
			10,
		},
		{

			[][]byte{
				[]byte("dc-end"),
				[]byte("HN-start"),
				[]byte("start-kj"),
				[]byte("dc-start"),
				[]byte("dc-HN"),
				[]byte("LN-dc"),
				[]byte("HN-end"),
				[]byte("kj-sa"),
				[]byte("kj-HN"),
				[]byte("kj-dc"),
			},
			19,
		},
		{
			[][]byte{
				[]byte("fs-end"),
				[]byte("he-DX"),
				[]byte("fs-he"),
				[]byte("start-DX"),
				[]byte("pj-DX"),
				[]byte("end-zg"),
				[]byte("zg-sl"),
				[]byte("zg-pj"),
				[]byte("pj-he"),
				[]byte("RW-he"),
				[]byte("fs-DX"),
				[]byte("pj-RW"),
				[]byte("zg-RW"),
				[]byte("start-pj"),
				[]byte("he-WI"),
				[]byte("zg-he"),
				[]byte("pj-fs"),
				[]byte("start-RW"),
			},
			226,
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
				[]byte("start-A"),
				[]byte("start-b"),
				[]byte("A-c"),
				[]byte("A-b"),
				[]byte("b-d"),
				[]byte("A-end"),
				[]byte("b-end"),
			},
			36,
		},
		{

			[][]byte{
				[]byte("dc-end"),
				[]byte("HN-start"),
				[]byte("start-kj"),
				[]byte("dc-start"),
				[]byte("dc-HN"),
				[]byte("LN-dc"),
				[]byte("HN-end"),
				[]byte("kj-sa"),
				[]byte("kj-HN"),
				[]byte("kj-dc"),
			},
			103,
		},
		{
			[][]byte{
				[]byte("fs-end"),
				[]byte("he-DX"),
				[]byte("fs-he"),
				[]byte("start-DX"),
				[]byte("pj-DX"),
				[]byte("end-zg"),
				[]byte("zg-sl"),
				[]byte("zg-pj"),
				[]byte("pj-he"),
				[]byte("RW-he"),
				[]byte("fs-DX"),
				[]byte("pj-RW"),
				[]byte("zg-RW"),
				[]byte("start-pj"),
				[]byte("he-WI"),
				[]byte("zg-he"),
				[]byte("pj-fs"),
				[]byte("start-RW"),
			},
			3509,
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

func TestParseInput(t *testing.T) {
	expected := map[string][]string{
		"start": {"A", "b"},
		"A":     {"start", "c", "b", "end"},
		"b":     {"start", "A", "d", "end"},
		"c":     {"A"},
		"d":     {"b"},
		"end":   {"A", "b"},
	}
	lines := [][]byte{
		[]byte("start-A"),
		[]byte("start-b"),
		[]byte("A-c"),
		[]byte("A-b"),
		[]byte("b-d"),
		[]byte("A-end"),
		[]byte("b-end"),
	}

	result := ParseInput(lines)
	if !reflect.DeepEqual(result, expected) {
		t.Fatalf("Result is not equal to expected, got: %v, want %v\n", result, expected)
	}
}
