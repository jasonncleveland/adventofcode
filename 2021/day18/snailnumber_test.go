package day18

import (
	"os"
	"testing"
)

func TestToString(t *testing.T) {
	root := &SnailNumber{
		Value: -1,
		Side:  Root,
		Left: &SnailNumber{
			Value: -1,
			Side:  LeftSide,
			Left:  &SnailNumber{Value: 1, Side: LeftSide},
			Right: &SnailNumber{Value: 2, Side: RightSide},
		},
		Right: &SnailNumber{
			Value: -1,
			Side:  RightSide,
			Left: &SnailNumber{
				Value: -1,
				Side:  LeftSide,
				Left:  &SnailNumber{Value: 3, Side: LeftSide},
				Right: &SnailNumber{Value: 4, Side: RightSide},
			},
			Right: &SnailNumber{Value: 5, Side: RightSide},
		},
	}
	expected := "[[1,2],[[3,4],5]]"

	result := root.ToString()
	if result != expected {
		t.Errorf("Result is not equal to expected, got: %s, want %s\n", result, expected)
	}

	// Test the case where the given node is nil
	var rootNil *SnailNumber
	resultNil := rootNil.ToString()
	expectedNil := "[]"
	if resultNil != expectedNil {
		t.Errorf("Result is not equal to expected, got: %s, want %s\n", resultNil, expectedNil)
	}
}

func TestToStringColoured(t *testing.T) {
	root := &SnailNumber{
		Value: -1,
		Side:  Root,
		Left: &SnailNumber{
			Value: -1,
			Side:  LeftSide,
			Left:  &SnailNumber{Value: 1, Side: LeftSide},
			Right: &SnailNumber{Value: 2, Side: RightSide},
		},
		Right: &SnailNumber{
			Value: -1,
			Side:  RightSide,
			Left: &SnailNumber{
				Value: -1,
				Side:  LeftSide,
				Left:  &SnailNumber{Value: 3, Side: LeftSide},
				Right: &SnailNumber{Value: 4, Side: RightSide},
			},
			Right: &SnailNumber{Value: 5, Side: RightSide},
		},
	}
	expected := "\033[1;31m[\033[0m\033[1;35m[\033[0m\033[1;35m1\033[0m\033[1;35m,\033[0m\033[1;36m2\033[0m\033[1;35m]\033[0m\033[1;31m,\033[0m\033[1;36m[\033[0m\033[1;35m[\033[0m\033[1;35m3\033[0m\033[1;35m,\033[0m\033[1;36m4\033[0m\033[1;35m]\033[0m\033[1;36m,\033[0m\033[1;36m5\033[0m\033[1;36m]\033[0m\033[1;31m]\033[0m"

	// Set the debug flag to true to enable coloured text
	os.Setenv("AOC_DEBUG", "true")
	defer os.Clearenv()

	result := root.ToString()
	if result != expected {
		t.Errorf("Result is not equal to expected, got: %s, want %s\n", result, expected)
	}
}

func TestExplode(t *testing.T) {
	tests := []struct {
		input    []byte
		expected []byte
	}{
		{
			[]byte("[[[[[9,8],1],2],3],4]"),
			[]byte("[[[[0,9],2],3],4]"),
		},
		{
			[]byte("[7,[6,[5,[4,[3,2]]]]]"),
			[]byte("[7,[6,[5,[7,0]]]]"),
		},
		{
			[]byte("[[6,[5,[4,[3,2]]]],1]"),
			[]byte("[[6,[5,[7,0]]],3]"),
		},
		{
			[]byte("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]"),
			[]byte("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]"),
		},
		{
			[]byte("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]"),
			[]byte("[[3,[2,[8,0]]],[9,[5,[7,0]]]]"),
		},
	}

	for _, test := range tests {
		node := Parse(test.input)
		node.Explode(0)

		result := node.ToString()
		if result != string(test.expected) {
			t.Errorf("Result was incorrect, got: %s, want: %s.\n", result, test.expected)
		}
	}
}

func TestSplit(t *testing.T) {
	tests := []struct {
		input    *SnailNumber
		expected []byte
	}{
		{
			&SnailNumber{
				Value: -1,
				Left: &SnailNumber{
					Value: 1,
				},
				Right: &SnailNumber{
					Value: 0,
				},
			},
			[]byte("[1,0]"),
		},
		{
			&SnailNumber{
				Value: -1,
				Left: &SnailNumber{
					Value: 10,
				},
				Right: &SnailNumber{
					Value: 0,
				},
			},
			[]byte("[[5,5],0]"),
		},
		{
			&SnailNumber{
				Value: -1,
				Left: &SnailNumber{
					Value: 0,
				},
				Right: &SnailNumber{
					Value: 10,
				},
			},
			[]byte("[0,[5,5]]"),
		},
		{
			&SnailNumber{
				Value: -1,
				Left: &SnailNumber{
					Value: 11,
				},
				Right: &SnailNumber{
					Value: 0,
				},
			},
			[]byte("[[5,6],0]"),
		},
		{
			&SnailNumber{
				Value: -1,
				Left: &SnailNumber{
					Value: 12,
				},
				Right: &SnailNumber{
					Value: 0,
				},
			},
			[]byte("[[6,6],0]"),
		},
	}

	for _, test := range tests {
		test.input.Split()

		result := test.input.ToString()
		if result != string(test.expected) {
			t.Errorf("Result was incorrect, got: %s, want: %s.\n", result, test.expected)
		}
	}
}

func TestReduce(t *testing.T) {
	tests := []struct {
		input    []byte
		expected []byte
	}{
		{
			[]byte("[[[[[4,3],4],4],[7,[[8,4],9]]],[1,1]]"),
			[]byte("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]"),
		},
	}

	for _, test := range tests {
		node := Parse(test.input)

		node.Reduce()
		result := node.ToString()
		if result != string(test.expected) {
			t.Errorf("Result was incorrect given input %s, got: %s, want: %s.\n", test.input, result, test.expected)
		}
	}
}

func TestMagnitude(t *testing.T) {
	tests := []struct {
		input    []byte
		expected int64
	}{
		{
			[]byte("[9,1]"),
			29,
		},
		{
			[]byte("[1,9]"),
			21,
		},
		{
			[]byte("[[9,1],[1,9]]"),
			129,
		},
		{
			[]byte("[[1,2],[[3,4],5]]"),
			143,
		},
		{
			[]byte("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]"),
			1384,
		},
		{
			[]byte("[[[[1,1],[2,2]],[3,3]],[4,4]]"),
			445,
		},
		{
			[]byte("[[[[3,0],[5,3]],[4,4]],[5,5]]"),
			791,
		},
		{
			[]byte("[[[[5,0],[7,4]],[5,5]],[6,6]]"),
			1137,
		},
		{
			[]byte("[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]"),
			3488,
		},
		{
			[]byte("[[[[6,6],[7,6]],[[7,7],[7,0]]],[[[7,7],[7,7]],[[7,8],[9,9]]]]"),
			4140,
		},
	}

	for _, test := range tests {
		node := Parse(test.input)

		result := node.Magnitude()
		if result != test.expected {
			t.Errorf("Result was incorrect given input %s, got: %d, want: %d.\n", test.input, result, test.expected)
		}
	}
}

func TestParse(t *testing.T) {
	expected := &SnailNumber{
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
	}
	line := []byte("[[1,2],[[3,4],5]")

	result := Parse(line)
	if result.ToString() != expected.ToString() {
		t.Errorf("Result is not equal to expected, got: %s, want %s\n", result.ToString(), expected.ToString())
	}
}

func TestAdd(t *testing.T) {
	tests := []struct {
		input    [][]byte
		expected string
	}{
		{
			[][]byte{
				[]byte("[1,1]"),
				[]byte("[2,2]"),
				[]byte("[3,3]"),
				[]byte("[4,4]"),
			},
			"[[[[1,1],[2,2]],[3,3]],[4,4]]",
		},
		{
			[][]byte{
				[]byte("[1,1]"),
				[]byte("[2,2]"),
				[]byte("[3,3]"),
				[]byte("[4,4]"),
				[]byte("[5,5]"),
			},
			"[[[[3,0],[5,3]],[4,4]],[5,5]]",
		},
		{
			[][]byte{
				[]byte("[1,1]"),
				[]byte("[2,2]"),
				[]byte("[3,3]"),
				[]byte("[4,4]"),
				[]byte("[5,5]"),
				[]byte("[6,6]"),
			},
			"[[[[5,0],[7,4]],[5,5]],[6,6]]",
		},
		{
			[][]byte{
				[]byte("[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]"),
				[]byte("[7,[[[3,7],[4,3]],[[6,3],[8,8]]]]"),
				[]byte("[[2,[[0,8],[3,4]]],[[[6,7],1],[7,[1,6]]]]"),
				[]byte("[[[[2,4],7],[6,[0,5]]],[[[6,8],[2,8]],[[2,1],[4,5]]]]"),
				[]byte("[7,[5,[[3,8],[1,4]]]]"),
				[]byte("[[2,[2,2]],[8,[8,1]]]"),
				[]byte("[2,9]"),
				[]byte("[1,[[[9,3],9],[[9,0],[0,7]]]]"),
				[]byte("[[[5,[7,4]],7],1]"),
				[]byte("[[[[4,2],2],6],[8,7]]"),
			},
			"[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]",
		},
		{
			[][]byte{
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
			},
			"[[[[6,6],[7,6]],[[7,7],[7,0]]],[[[7,7],[7,7]],[[7,8],[9,9]]]]",
		},
	}

	for _, test := range tests {
		var total *SnailNumber
		for _, input := range test.input {
			node := Parse(input)
			total = Add(total, node)
		}

		result := total.ToString()
		if result != string(test.expected) {
			t.Errorf("Result was incorrect, got: %s, want: %s.\n", result, test.expected)
		}
	}
}
