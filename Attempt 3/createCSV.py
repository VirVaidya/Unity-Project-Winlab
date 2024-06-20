import csv

def parse_pdb_to_csv(pdb_filename, csv_filename):
    with open(pdb_filename, 'r') as pdb_file, open(csv_filename, 'w', newline='') as csv_file:
        csv_writer = csv.writer(csv_file)
        csv_writer.writerow(['Amino Acid', 'X', 'Y', 'Z', 'Displacement', 'Atom'])
        for line in pdb_file:
            if line.startswith('ATOM') or line.startswith('HETATM'):
                amino_acid = line[17:20].strip()
                x = float(line[30:38].strip())
                y = float(line[38:46].strip())
                z = float(line[46:54].strip())
                displacement = float(line[60:66].strip())
                atom = line[76:78].strip()
                csv_writer.writerow([amino_acid, x, y, z, displacement, atom])

parse_pdb_to_csv("C:/Users/native/Protein Visualization test/Assets/Input/5ott.pdb", "5ott.csv")

