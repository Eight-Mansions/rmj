CC		= ccpsx
ASM		= asmpsx
LINK	= slink
LIBLNK	= psylib2

ASM_REL=0
ifeq ($(RELMODE),DEBUG)
	ASM_REL=1
endif

CCOPTIONS  = -Wunused -O$(OPTIMIZE) -G0 -comments-c++ -nostdlib -c -D$(RELMODE) -I.. -I.
CCOOPTIONS = -O$(OPTIMIZE) -G0 -comments-c++ -nostdlib -c -DOVERLAY -D$(RELMODE)
ASMOPTIONS = /l /o c+ /o an+ /zd /x /e RELMODE=${ASM_REL}
LINKOPTS = /psx /c /z /li $(RELMODE)
LINKOOPTS = /psx /p /c /li $(RELMODE)

#CORE_SRC   = main.c controller.c qalloc.c text.c string.c gui.c list.c thread.c cd.c memory.c sound.c sseq.c
#CORE_S     = engine.asm
#FIELD_SRC  = char.c
#MENU_SRC   = menu.c root.c item.c ability.c equip.c status.c bestiary.c config.c save.c
#BATTLE_SRC = battle.c
SUBTITLE_SRC = subtitle.cpp generated_audio.cpp generated_movie.cpp initsubs.cpp


## build object names from types ##
#CORE_OBJ	= $(patsubst %.c, $(RELMODE)/%.obj, ${CORE_SRC})
#CORE_ASM	= $(patsubst %.asm, $(RELMODE)/%.obj, ${CORE_S})
#FIELD_OBJ	= $(patsubst %.c, $(RELMODE)/%.obf, ${FIELD_SRC})
#MENU_OBJ	= $(patsubst %.c, $(RELMODE)/%.obm, ${MENU_SRC})
#BATTLE_OBJ	= $(patsubst %.c, $(RELMODE)/%.obb, ${BATTLE_SRC})
SUBTITLE_OBJ = $(patsubst %.cpp, $(RELMODE)/%.obj, ${SUBTITLE_SRC})


## list of objects to use for linking ##
## NOTE: you still need to specify them in the ##
## .lk file or they will not be linked at all! ##
#ALL_SRC = $(CORE_OBJ) $(MENU_OBJ) $(BATTLE_OBJ) $(FIELD_OBJ)
#ALL_ASM = $(CORE_ASM)
ALL_SRC = $(SUBTITLE_OBJ)


#ALL_OBJ = $(ALL_SRC) $(ALL_ASM)
ALL_OBJ = $(ALL_SRC)
## ------- ##

#====================================#
# kernel objects to build ENGINE.BIN #
#====================================#
#$(RELMODE)/%.obj : core/%.c
#	$(CC) $(CCOPTIONS) $< -o $@
#$(RELMODE)/%.obj : core/%.asm
#	$(ASM) $(ASMOPTIONS) $<,$@
#================================#
# menu objects to build MENU.BIN #
#================================#
#$(RELMODE)/%.obm: menu/%.c
#	$(CC) $(CCOPTIONS) $< -o $@
#================================#
# battle objects				 #
#================================#
#$(RELMODE)/%.obb: battle/%.c
#	$(CC) $(CCOPTIONS) $< -o $@
#================================#
# field objects					 #
#================================#
#$(RELMODE)/%.obf: field/%.c
#	$(CC) $(CCOPTIONS) $< -o $@
#====================================#
# subtitle objects                   #
#====================================#
$(RELMODE)/%.obj: subtitle/%.cpp
	$(CC) $(CCOPTIONS) $< -o $@

all: inform $(RELMODE)/${OUTFILE}.cpe

inform:
ifneq ($(RELMODE),RELEASE)
ifneq ($(RELMODE),DEBUG)
	@echo "Invalid configuration "$(RELMODE)" specified."
	@echo "You must specify a configuration when running make, e.g."
	@echo  "make RELMODE=DEBUG"
	@echo  
	@echo  "Possible choices for configuration are 'RELEASE' and 'DEBUG'"
	@exit 1
endif
endif
	@echo "Configuration "$(RELMODE)
	@echo "------------------------"

$(RELMODE)/${OUTFILE}.cpe: ${ALL_OBJ}
	copy $(RELMODE)\*.o* obj\ /y

clean:
	rmdir /q /s $(RELMODE)

rebuild: clean | all

# DO NOT DELETE
