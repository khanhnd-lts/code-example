<template>
  <div
    class="quick-chat-container"
    :style="{
      'border-bottom-left-radius': borderStyle.bottomLeft,
      'border-bottom-right-radius': borderStyle.bottomRight,
      'border-top-right-radius': borderStyle.topRight,
      'border-top-left-radius': borderStyle.topLeft,
    }"
  >
    <Header
      v-if="displayHeader"
      :colors="colors"
      :border-style="borderStyle"
      :participants="participants"
      :myself="myself"
      :hide-close-button="hideCloseButton"
      :close-button-icon-size="closeButtonIconSize"
      @onClose="onClose()"
    >
      <template #header>
        <slot name="header"></slot>
      </template>
      <template #close-button>
        <slot name="close-button"></slot>
      </template>
    </Header>
    <MessageDisplay
      :colors="colors"
      :async-mode="asyncMode"
      :load-more-messages="loadMoreMessages"
      :scroll-bottom="scrollBottom"
      :profile-picture-config="profilePictureConfig"
      :timestamp-config="timestampConfig"
      :messages="messages"
      :participants="participants"
      :myself="myself"
      :loading-messages="loadingMessages"
      @onImageClicked="onImageClicked"
      @onLoadMoreMessages="onLoadMoreMessages"
    />
    <MessageManager
      :colors="colors"
      :myself="myself"
      :border-style="borderStyle"
      :submit-icon-size="submitIconSize"
      :submit-image-icon-size="submitImageIconSize"
      :send-images="sendImages"
      @onImageSelected="onImageSelected"
      @onMessageSubmit="onMessageSubmit"
      @onType="onType"
    />
    <ModalShowImage ref="modalShowImage" />
  </div>
</template>

<script lang="ts">
import { Vue } from "vue-property-decorator";
import Header from "./Header.vue";
import MessageDisplay from "./MessageDisplay.vue";
import MessageManager from "./MessageManager.vue";
import ModalShowImage from "./ModalShowImage.vue";
export default Vue.extend({
  name: "chat",
  components: {
    Header,
    MessageDisplay,
    MessageManager,
    ModalShowImage,
  },
  props: {
    participants: {
      type: Array,
      required: true,
    },
    messages: {
      type: [Array, Object],
      required: true,
    },
    myself: {
      type: Object,
      required: true,
    },
    chatTitle: {
      type: String,
      required: false,
      default: "",
    },
    placeholder: {
      type: String,
      required: false,
      default: "type your message here",
    },
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
    hideCloseButton: {
      type: Boolean,
      required: false,
      default: false,
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
    closeButtonIconSize: {
      type: String,
      required: false,
      default: "15px",
    },
    asyncMode: {
      type: Boolean,
      required: false,
      default: false,
    },
    loadMoreMessages: {
      type: Function,
      required: false,
      default: null,
    },
    scrollBottom: {
      type: Object,
      required: false,
      default: () => {
        return {
          messageSent: true,
          messageReceived: true,
        };
      },
    },
    displayHeader: {
      type: Boolean,
      required: false,
      default: true,
    },
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
    /* onClose: {
                type: Function,
                required: false,
                default: () => false
            }, */
    /* onImageSelected: {
                type: Function,
                required: false,
                default: () => false
            }, */
    /* onImageClicked: {
                type: Function,
                required: false,
                default: () => false
            }, */
    sendImages: {
      type: Boolean,
      required: false,
      default: true,
    },
    profilePictureConfig: {
      type: Object,
      required: false,
      default: () => {
        return {
          others: true,
          myself: false,
          styles: {
            width: "25px",
            height: "25px",
            borderRadius: "50%",
          },
        };
      },
    },
    timestampConfig: {
      type: Object,
      required: false,
      default: () => {
        return {
          format: "HH:mm",
          relative: false,
        };
      },
    },
    loadingMessages: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  data() {
    return {};
  },
  watch: {
    participants() {
      this.setParticipants(this.participants);
    },
    myself() {
      this.setMyself(this.myself);
    },
    messages() {
      this.setMessages(this.messages);
    },
    placeholder() {
      this.setPlaceholder(this.placeholder);
    },
    chatTitle() {
      this.setChatTitle(this.chatTitle);
    },
  },
  beforeCreate() {},
  created() {
    //
    this.setParticipants(this.participants);
    this.setMyself(this.myself);
    this.setMessages(this.messages);
    this.setPlaceholder(this.placeholder);
    this.setChatTitle(this.chatTitle);
  },
  methods: {
    setParticipants(e: any) {},
    setMyself(e: any) {},
    setMessages(e: any) {},
    setPlaceholder(e: any) {},
    setChatTitle(e: any) {},
    onClose: function () {
      this.$emit("onClose");
    },
    onType: function (e: any) {
      this.$emit("onType", e);
    },
    onMessageSubmit: function (message: any) {
      this.$emit("onMessageSubmit", message);
    },
    onImageSelected: function (data: any) {
      this.$emit("onImageSelected", data);
    },
    onImageClicked: function (message: any) {
      this.$emit("onImageClicked", message);
      (this.$refs.modalShowImage as any).show(message);
    },
    onLoadMoreMessages() {
      this.$emit("onLoadMoreMessages");
    },
  },
});
</script>

<style>
.quick-chat-container {
  display: flex;
  width: 100%;
  height: 100%;
  background: #f0eeee;
  flex-direction: column;
  align-items: stretch;
  overflow: hidden;
  z-index: 9;
  box-shadow: 0 12px 28px 0 rgba(0, 0, 0, 0.2), 0 2px 4px 0 rgba(0, 0, 0, 0.1);
}
</style>
