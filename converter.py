

import binascii

def toString():
    filename = "c:\\temp\\file"
    with open(filename, 'rb') as f:
        content = f.read()
    print len(content)
    
    filename = "c:\\temp\\result"
    with open(filename, 'wb') as f:
        f.write(binascii.hexlify(content))
    print "Store done!"


def fromString():
    filename = "c:\\temp\\result"
    with open(filename, 'rb') as f:
        content = f.read()
    print len(content)
    
    filename = "c:\\temp\\restored"
    with open(filename, 'wb') as f:
        f.write(binascii.unhexlify(content))
    print "Restore done!"

def main():
    fromString()
    print "done"
