import os

def process(filename):
    file = open(filename, 'r')
    p1wins = 0
    p2wins = 0
    ties = 0
    print(filename)
    lastLine = None
    for line in file.readlines():
        lastLine = line
        arr = line.split(' ')
        outcome = arr[3]
        if (outcome == 'player1_win'):
            p1wins += 1
        if (outcome == 'player2_win'):
            p2wins += 1
        if (outcome == 'Tie'):
            ties += 1
    print(p1wins, p2wins, ties)
    print(lastLine)
        
for filename in os.listdir(os.getcwd()):
    if filename.endswith(".txt") and ('_' in filename): 
        process(filename)
        continue
    else:
        continue
    
