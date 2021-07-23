<template>
        <v-dialog v-model="dialog"
        fullscreen
        class="primary"
                  max-width="100%">
                <v-img :src="HOST + '/api/fileupload/download/' + message.photo" 
                max-height="100%"
                aspect-ratio="1"
              class="grey"
                contain></v-img>
                <v-btn
              fixed
              dark
              fab
              top
              right
              small
              color="black"
              @click="hide"
            >
              <v-icon>close</v-icon>
            </v-btn>
        </v-dialog>
</template>

<script lang="ts">
    import { Vue } from 'vue-property-decorator';
    export default Vue.extend({
        components: {
        },
        data() {
            return {
                HOST: process.env.VUE_APP_ROOT_API,
                dialog: false,
                message: {} as any
            }
        },
        methods: {
            show(message: any) {
                this.message = message;
                this.dialog = true;
            },
            hide() {
                this.dialog = false;
            }
        },
    });
</script>
