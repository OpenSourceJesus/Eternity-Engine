import os, sys, sysconfig, subprocess
from pathlib import *

_thisDir = os.path.split(os.path.abspath(__file__))[0]
if not os.path.isdir('PyRapier2d'):
	os.system('git clone https://github.com/OpenSourceJesus/PyRapier2d --depth=1')
os.system('''sudo apt install python3-pip
/bin/bash -c "$(curl -fsSL https://exaloop.io/install.sh)"
pip install codon-jit
pip install codon-jit --break-system-packages
pip install maturin
pip install maturin --break-system-packages
pip install pygame
pip install pygame --break-system-packages
sudo apt update
sudo apt install patchelf
sudo dnf install patchelf
sudo apt install rustc
sudo apt install cargo
cd PyRapier2d
maturin build --release''')
filesAndDirs = os.listdir(_thisDir + '/PyRapier2d/target/wheels')
os.system('pip install PyRapier2d/target/wheels/' + filesAndDirs[0] + '\npip install PyRapier2d/target/wheels/' + filesAndDirs[0] + ' --break-system-packages')
try:
	pythonLibDir = Path(sysconfig.get_config_var('LIBDIR'))
	pythonLibSoName = sysconfig.get_config_var('LDLIBRARY')
	codonLibDr = Path.home() / ".codon" / "lib" / "codon"
	if not all([pythonLibDir.is_dir(), codonLibDr.is_dir(), pythonLibSoName]):
		raise ValueError("One or more library paths could not be determined.")
except (TypeError, ValueError) as e:
	sys.exit(1)
genericSymlink = pythonLibDir / "libpython.so"
if not genericSymlink.exists():
	try:
		if is_interactive():
			response = input("\n     Do you want to authorize this command? (y/N) ")
			if response.lower() not in ['y', 'yes']:
				sys.exit(1)
		cmd = ["sudo", "ln", "-s", pythonLibSoName, "libpython.so"]
		subprocess.run(cmd, cwd = pythonLibDir, check = True)
	except (subprocess.CalledProcessError, FileNotFoundError) as e:
		if is_interactive():
			sys.exit(1)
	except KeyboardInterrupt:
		sys.exit(1)