package day18

import (
	"testing"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]"),
		[]byte("[[[5,[2,8]],4],[5,[[9,9],0]]]"),
		[]byte("[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]"),
		[]byte("[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]"),
		[]byte("[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]"),
		[]byte("[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]"),
		[]byte("[[[[5,4],[7,7]],8],[[8,3],8]]"),
		[]byte("[[9,3],[[9,9],[6,[4,9]]]]"),
		[]byte("[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]"),
		[]byte("[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(4140)
	if result != expected {
		t.Errorf("Result was incorrect, got: %v, want: %v.\n", result, expected)
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

func TestParseInput(t *testing.T) {
	expected := []*SnailNumber{
		{
			Value: -1,
			Left: &SnailNumber{
				Value: -1,
				Left:  &SnailNumber{Value: 1},
				Right: &SnailNumber{Value: 2},
			},
			Right: &SnailNumber{
				Value: -1,
				Left: &SnailNumber{
					Value: -1,
					Left:  &SnailNumber{Value: 3},
					Right: &SnailNumber{Value: 4},
				},
				Right: &SnailNumber{Value: 5},
			},
		},
	}
	lines := [][]byte{
		[]byte("[[1,2],[[3,4],5]"),
	}

	result := ParseInput(lines)
	if len(result) != len(expected) {
		t.Fatalf("Length of result is not equal to the expected length, got: %d, want %d\n", len(result), len(expected))
	}
	for i := range result {
		if result[i].ToString() != expected[i].ToString() {
			t.Errorf("Result is not equal to expected, got: %s, want %s\n", result[i].ToString(), expected[i].ToString())
		}
	}
}
