<template>
  <div style="background: #ffffff">
    <div class="container-message-manager pa-2">
      <div class="message-text-box">
        <div
          ref="userInput"
          class="message-input"
          :placeholder="placeholder"
          tabindex="0"
          contenteditable="true"
          @input="handleType"
          @keyup.enter.exact="sendMessage"
        ></div>
      </div>
      <!--<div class="container-send-message icon-send-message" @click.prevent="sendMessage">
                <v-icon :size="submitIconSize" :color="colors.submitIcon">send</v-icon>
      </div>-->
      <!--<div v-if="sendImages" class="container-send-message icon-send-message" @click="pickImage">
                <input ref="inputImage" accept="image/*" type="file" style="display: none;" @input="handleImageChange">
                <v-icon :size="submitIconSize" :color="colors.submitIcon">insert_photo</v-icon>
      </div>-->
      <div class="message-funtions">
        <v-btn text icon small v-if="sendImages" @click="pickImage">
          <input
            ref="inputImage"
            id="inputImage"
            accept="image/*"
            type="file"
            style="display: none"
            @input="handleImageChange"
          />
          <v-icon :color="colors.submitIcon"> mdi-file-image </v-icon>
        </v-btn>
        <v-menu
          v-model="menuEmoji"
          :close-on-content-click="false"
          :nudge-width="200"
          top
          right
          offset-y
        >
          <template v-slot:activator="{ on, attrs }">
            <v-btn text icon small v-bind="attrs" v-on="on">
              <v-icon :color="colors.submitIcon">mdi-sticker-emoji</v-icon>
            </v-btn>
          </template>
          <VEmojiPicker @select="selectEmoji" />
        </v-menu>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
//import 'vue-material-design-icons/styles.css';
import { mapMutations } from "vuex";
import { Vue } from "vue-property-decorator";
import { VEmojiPicker } from "v-emoji-picker";
export default Vue.extend({
  components: {
    VEmojiPicker,
  },
  props: {
    /* onType: {
                type: Function,
                required: false,
                default: () => false
            }, */
    /* onMessageSubmit: {
                type: Function,
                required: false,
                default: () => false
            }, */
    colors: {
      type: Object,
      required: true,
    },
    borderStyle: {
      type: Object,
      required: false,
      default: () => {
        return {
          topLeft: "10px",
          topRight: "10px",
          bottomLeft: "10px",
          bottomRight: "10px",
        };
      },
    },
    submitIconSize: {
      type: Number,
      required: false,
      default: 22,
    },
    submitImageIconSize: {
      type: Number,
      required: false,
      default: 22,
    },
    /* onImageSelected: {
                type: Function,
                required: false,
                default: () => false
            }, */
    sendImages: {
      type: Boolean,
      required: false,
      default: true,
    },
    myself: {
      type: Object,
      required: false,
      default: {},
    },
  },
  data() {
    return {
      menuEmoji: false,
      textInput: "",
    };
  },
  computed: {
    placeholder(): any {
      return "Nhập nội dung";
    },
  },
  methods: {
    sendMessage(e: any) {
      this.textInput = (this.$refs.userInput as any).textContent;
      (this.$refs.userInput as any).textContent = "";

      if (this.textInput && this.textInput.trim() != "") {
        let inputLen = this.textInput.length;
        let inputText = this.textInput;
        if (this.textInput[inputLen - 1] === "\n") {
          inputText = inputText.slice(0, inputLen - 1);
        }
        let message = {
          content: inputText,
          // myself: true,
          participantId: this.myself.id,
          timestamp: this.$moment(),
          loading: true,
          viewed: false,
          type: "text",
        };
        this.$emit("onMessageSubmit", message);
        //this.onMessageSubmit(message);
      }
    },
    handleType: function (e: any) {
      this.$emit("onType", e);
    },
    pickImage: function () {
      (this.$refs.inputImage as any).click();
    },
    handleImageChange: async function (e: any) {
      const files = e.target.files;
      this.textInput = (this.$refs.userInput as any).textContent;
      (this.$refs.userInput as any).textContent = "";
      let inputLen = this.textInput.length;
      let inputText = this.textInput;
      if (this.textInput[inputLen - 1] === "\n") {
        inputText = inputText.slice(0, inputLen - 1);
      }
      let message = {
        photo: URL.createObjectURL(files[0]),
        content: inputText,
        participantId: this.myself.id,
        timestamp: this.$moment(),
        loading: true,
        viewed: false,
      };
      this.$emit("onImageSelected", { file: files[0], message });
      //this.onImageSelected(files, message)
    },
    selectEmoji(emoji: any) {
      //   console.log(emoji);
      (this.$refs.userInput as any).textContent += emoji.data;
      this.menuEmoji = false;
    },
  },
});
</script>

<style>
.quick-chat-container .container-message-manager {
  border-top: 1px solid #dee2e6;
  background: #fff;
  display: flex;
  align-items: center;
  padding: 0;
  /* -webkit-box-shadow: 0px -2px 40px 0px rgba(186, 186, 186, 0.67);
  box-shadow: 0px -2px 40px 0px rgba(186, 186, 186, 0.67); */
  overflow: hidden;
}
.quick-chat-container .message-funtions {
  background-color: #fff;
  margin-left: 4px;
}
.quick-chat-container .container-message-manager .message-text-box {
  display: flex;
  align-items: center;
  flex: 1;
  overflow: hidden;
  width: 100%;
  resize: none;
  border: 1px solid rgb(200, 200, 200);
  border-radius: 20px;
  background: #f0f2f5;
  outline: none;
  min-height: 37px;
  max-height: 100px;
  box-sizing: border-box;
  -webkit-font-smoothing: antialiased;
  padding: 4px 10px;
}

.quick-chat-container .container-message-manager .message-input {
  width: 100%;
  outline: none;
  font-size: 15px;
  font-weight: 400;
  line-height: 1.33;
  white-space: pre-wrap;
  word-wrap: break-word;
  color: #565867;
  -webkit-font-smoothing: antialiased;
  overflow: scroll;
  overflow-x: hidden;
  overflow-y: auto;
  cursor: text;
}

.quick-chat-container .container-message-manager .message-input:empty:before {
  content: attr(placeholder);
  display: block;
  filter: contrast(15%);
  outline: none;
}

.quick-chat-container .container-message-manager .message-input:focus {
  outline: none;
}

/* .quick-chat-container .container-message-manager   .container-send-message {
            margin-left: 10px;
        } */

.quick-chat-container .container-message-manager .container-send-message svg {
  -webkit-box-sizing: content-box;
  box-sizing: content-box;
}

.quick-chat-container .container-message-manager .icon-send-message {
  cursor: pointer;
  opacity: 0.7;
  transition: 0.3s;
  border-radius: 11px;
  padding: 8px;
}

.quick-chat-container .container-message-manager .icon-send-message:hover {
  opacity: 1;
  background: #eee;
}
</style>
