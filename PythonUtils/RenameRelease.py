import os

remote_folder = "Y:\\1525\\"
debug_exe_name = remote_folder + "1525L29U020D.exe"
release_exe_name = remote_folder + "1525L29U020R.exe"


if os.access(release_exe_name, os.F_OK):
	print ("File " + release_exe_name + " Exists. Removing.")
	os.remove(release_exe_name)
else:
	print ("File Does Not Exist")

try:
	os.rename(debug_exe_name, release_exe_name)
	print ("File successfully renamed to " + release_exe_name)
except OSError:
	print ("OSError thrown")
except FileExistsError:
	print ("File Exists Error")

