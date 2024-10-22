
SOURCES := $(wildcard src/*.adoc)
BUILD_DIR := build

BASENAMES := $(notdir $(SOURCES))

HTML_OUT := $(addprefix $(BUILD_DIR)/, $(BASENAMES:.adoc=.html))
PDF_OUT := $(addprefix $(BUILD_DIR)/, $(BASENAMES:.adoc=.pdf))
MD_OUT := $(addprefix $(BUILD_DIR)/, $(BASENAMES:.adoc=.md))

# Default target                                                                                                                                                                                                                              
all: html pdf md

html: $(HTML_OUT)

pdf: $(PDF_OUT)

md: $(MD_OUT)

$(BUILD_DIR)/%.html: src/%.adoc
	@mkdir -p $(BUILD_DIR)
	@sudo docker run --rm -e TZ=America/New_York -v $(PWD):/documents asciidoctor/docker-asciidoctor asciidoctor -D /documents/$(BUILD_DIR) $<

$(BUILD_DIR)/%.pdf: src/%.adoc
	@mkdir -p $(BUILD_DIR)
	@sudo docker run --rm -e TZ=America/New_York -v $(PWD):/documents asciidoctor/docker-asciidoctor asciidoctor-pdf -D /documents/$(BUILD_DIR) $<

$(BUILD_DIR)/%.md: src/%.adoc
	@mkdir -p $(BUILD_DIR)
	@sudo docker run --rm -e TZ=America/New_York -v $(PWD):/documents asciidoctor/docker-asciidoctor asciidoctor -D /documents/$(BUILD_DIR) -b docbook $<
	@sudo docker run --rm -e TZ=America/New_York -v $(PWD):/data pandoc/core -f docbook -t markdown -s $(BUILD_DIR)/$*.xml -o $(BUILD_DIR)/$*.md
	@rm -f $(BUILD_DIR)/$*.xml

clean:
	@rm -rf $(BUILD_DIR)

.PHONY: all html pdf clean
