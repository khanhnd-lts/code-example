<template>
  <div
    ref="containerMessageDisplay"
    :style="{ background: colors.message.messagesDisplay.bg }"
    class="container-message-display"
    @scroll="updateScrollState"
  >
    <div v-if="loadingMessages" class="loader text-center mt-4">
      <div class="message-loading"></div>
    </div>
    <div
      v-for="(message, index) in messages"
      :key="index"
      class="message-container"
    >
      <MyMessage
        v-if="message.creator.account == myself.account"
        :message="message"
        :async-mode="asyncMode"
        :colors="colors"
        :profile-picture-config="profilePictureConfig"
        :timestamp-config="timestampConfig"
        :myself="myself"
        @onImageClicked="onImageClicked"
      />
      <OtherMessage
        v-else
        :message="message"
        :async-mode="asyncMode"
        :colors="colors"
        :profile-picture-config="profilePictureConfig"
        :timestamp-config="timestampConfig"
        @onImageClicked="onImageClicked"
      />
    </div>
  </div>
</template>

<script lang="ts">
import { mapGetters, mapMutations } from "vuex";
import MyMessage from "./MyMessage.vue";
import OtherMessage from "./OtherMessage.vue";
import { Vue } from "vue-property-decorator";
export default Vue.extend({
  components: {
    MyMessage,
    OtherMessage,
  },
  props: {
    colors: {
      type: Object,
      required: true,
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
    /* onImageClicked: {
                type: Function,
                required: false,
                default: null
            }, */
    scrollBottom: {
      type: Object,
      required: true,
    },
    profilePictureConfig: {
      type: Object,
      required: true,
    },
    timestampConfig: {
      type: Object,
      required: true,
    },
    messages: {
      type: [Array, Object],
      required: false,
      default: [],
    },
    myself: {
      type: Object,
      required: false,
      default: {},
    },
    loadingMessages: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  data() {
    return {
      updateScroll: true,
      lastMessage: null as any,
    };
  },
  computed: {},
  mounted() {
    this.goToBottom();
    (this.$refs.containerMessageDisplay as any).dispatchEvent(
      new CustomEvent("scroll")
    );
  },
  watch: {
    messages(newVal, oldVal) {
      if (
        newVal &&
        newVal.length &&
        !this.messageCompare(newVal[newVal.length - 1], this.lastMessage)
      ) {
        if (
          this.updateScroll ||
          (this.scrollBottom.messageSent &&
            newVal[newVal.length - 1].creator.account == this.myself.account) ||
          (this.scrollBottom.messageReceived &&
            newVal[newVal.length - 1].creator.account != this.myself.account)
        ) {
          this.goToBottom();
          // if (newVal.length) {
          // this.lastMessage = newVal[newVal.length - 1]
          // }
        }
      }
    },
  },
  updated() {},
  methods: {
    /**
     * This function compare two messages without looking at the uploaded propertie.
     * This function has been implemented to prevent chat scrolling down after changing the message from 'uploaded = false' to 'uploaded = true'.
     * @param {Object} message1 the first message object
     * @param {Object} message2 the second message object
     * @return {Boolean} true if the messages are equal and false if they are different
     */
    messageCompare(message1: any, message2: any) {
      /**
       * if one of the messages are null, you can safely compare the messages with '==='
       */
      if (!message2 || !message1) {
        return message1 === message2;
      }
      /**
       * compare the immutable properties of a message
       */
      let participant_equal = message1.creatorId == message2.creatorId;
      let content_equal = message1.content == message2.content;
      let timestamp_equal = message1.createdTime === message2.createdTime;

      return participant_equal && content_equal && timestamp_equal;
    },
    updateScrollState(arg: any) {
      this.updateScroll =
        arg.target.scrollTop + arg.target.clientHeight >=
        arg.target.scrollHeight;

      if (
        //typeof this.loadMoreMessages === 'function' &&
        arg.target.scrollTop < 20
      ) {
        this.$emit("onLoadMoreMessages");
        // this.loadingMessages = true;
        // this.loadMoreMessages((messages: any) => {
        //     //if (Array.isArray(messages) && messages.length > 0) {
        //         /**
        //          * this code will be removed before the next release
        //          *
        //          * this line is commented because the setMessages is already called
        //          * when 'this.messages.unshift(...this.toLoad)' is executed at App.vue line 177
        //          * it was executing the same function twice, causing unexpected behavior with Luxon date objects
        //         */
        //         //this.setMessages([...messages, ...this.messages]);
        //     //}
        //     this.loadingMessages = false;
        // });
      }
    },
    goToBottom() {
      let scrollDiv = this.$refs.containerMessageDisplay as any;
      scrollDiv.scrollTop = scrollDiv.scrollHeight;

      this.updateScroll = false;
      setTimeout(() => {
        let scrollDiv = this.$refs.containerMessageDisplay as any;
        scrollDiv.scrollTop = scrollDiv.scrollHeight;
        this.updateScroll = false;
      }, 200);
    },
    onImageClicked(message: any) {
      this.$emit("onImageClicked", message);
    },
  },
});
</script>

<style>
.quick-chat-container .container-message-display {
  -webkit-box-flex: 1;
  -ms-flex: 1;
  flex: 1;
  -webkit-flex: 1;
  overflow-y: scroll;
  overflow-x: hidden;
  display: -webkit-box;
  display: -ms-flexbox;
  display: flex;
  -webkit-box-orient: vertical;
  -webkit-box-direction: normal;
  -ms-flex-direction: column;
  flex-direction: column;
  padding-bottom: 10px;
  max-height: 100%;
  padding-top: 8px;
}
/************** Safari 10.1+ ********************/
@media not all and (min-resolution: 0.001dpcm) {
  @supports (-webkit-appearance: none) {
    .message-container {
      display: -webkit-box !important;
    }
  }
}

.quick-chat-container .container-message-display .message-image {
  padding: 6px 0;
  border-radius: 15px;
  margin: 5px 0 5px 0;
  max-width: 100%;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
}

.quick-chat-container .container-message-display .message-image-display {
  width: 100%;
  border-radius: 18px;
  cursor: pointer;
  transition: 0.3s ease;
  border: 1px solid rgb(200, 200, 200);
}

.quick-chat-container .container-message-display .message-image-display:hover {
  opacity: 0.8;
}

.quick-chat-container .container-message-display .message-text > p {
  margin: 0;
  font-size: 15px;
}

.quick-chat-container
  .container-message-display
  .my-message
  > .message-timestamp {
  text-align: right;
}

/* .my-message {
            justify-content: flex-end;
            padding-right: 15px;
            align-items: flex-end;
        } */

.quick-chat-container .container-message-display .other-message {
  justify-content: flex-start;
  padding-left: 15px;
  align-items: flex-start;
}

.quick-chat-container
  .container-message-display
  .other-message
  > .message-text {
  color: #fff;
  border-bottom-left-radius: 0;
}

.quick-chat-container .container-message-display .my-message > .message-text {
  border-bottom-right-radius: 0;
}

.quick-chat-container .container-message-display .message-container {
  display: flex;
  /* flex-wrap: wrap; */
  flex-direction: column;
}

.quick-chat-container .container-message-display .message-username {
  font-size: 10px;
  font-weight: bold;
}

.quick-chat-container .container-message-display .icon-sent {
  padding-left: 5px;
  color: rgb(129, 127, 127);
}

.quick-chat-container .container-message-display .message-loading {
  height: 8px;
  width: 8px;
  border: 1px solid rgb(187, 183, 183);
  border-left-color: rgb(59, 59, 59);
  border-radius: 50%;
  margin-left: 5px;
  display: inline-block;
  animation: spin 1.3s ease infinite;
}

.quick-chat-container .container-message-display .loader .message-loading {
  width: 16px;
  height: 16px;
  margin: 5px 0 0 0;
}

.quick-chat-container .container-message-display .img-loading {
  height: 20px;
  width: 20px;
  border: 3px solid #ffffff00;
  border-left-color: #847f7f;
  border-top-color: #847f7f;
  border-radius: 50%;
  margin-left: 5px;
  display: inline-block;
  -webkit-animation: spin 1s ease infinite;
  animation: spin 1s ease infinite;
  position: absolute;
}

.quick-chat-container .container-message-display .img-overlay {
  opacity: 0.4;
}

.quick-chat-container .container-message-display .message-username-image {
  margin: 10px 10px 0px 10px;
  font-size: 12px;
  font-weight: bold;
}
@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}
</style>
