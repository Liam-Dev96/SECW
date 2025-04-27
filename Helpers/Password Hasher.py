import bcrypt

def hash_password(plain_password: str) -> str:
    # Generate salt and hash the password
    salt = bcrypt.gensalt(rounds=12)
    hashed = bcrypt.hashpw(plain_password.encode('utf-8'), salt)
    return hashed.decode('utf-8')

#this is the hashed password for the admin user.
admin_password = "Rxvstbvy@p72"
hashed_password = hash_password(admin_password)

print("BCrypt Hashed Password:\n" + hashed_password)


#function used to hash the admin password and update it in the database
#faster than having to alter the password in the database based upon some conditions